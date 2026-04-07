#!/usr/bin/env node
/**
 * encrypt-resume.js
 *
 * Encrypts the resume content in deploy/resume.html using AES-256-GCM.
 * The passkey is used as the encryption key material via PBKDF2.
 *
 * Usage:
 *   node encrypt-resume.js <passkey>
 *
 * This replaces the plaintext resume HTML with an encrypted base64 blob.
 * The browser decrypts it client-side only when the correct passkey is entered.
 * View-source shows only ciphertext. No server required.
 *
 * Security note: The decrypted content is Lance's own resume HTML, authored
 * by him, not untrusted user input. innerHTML is used intentionally to render
 * the decrypted markup. There is no XSS risk because the encrypted payload
 * is created by this build script from a known-safe source file.
 */

const crypto = require('crypto');
const fs = require('fs');
const path = require('path');

const passkey = process.argv[2];
if (!passkey) {
  console.error('Usage: node encrypt-resume.js <passkey>');
  process.exit(1);
}

const resumePath = path.join(__dirname, 'deploy', 'resume.html');
const html = fs.readFileSync(resumePath, 'utf8');

// Extract the inner content of .resume-content
const startMarker = '<div class="resume-content" id="resumeContent">';
const endMarker = '</div><!-- /resume-content -->';
const startIdx = html.indexOf(startMarker);
const endIdx = html.indexOf(endMarker);

if (startIdx === -1 || endIdx === -1) {
  console.error('Could not find resume-content markers in resume.html');
  process.exit(1);
}

const plaintext = html.substring(startIdx + startMarker.length, endIdx);

// Verify passkey matches the stored hash
const hash = crypto.createHash('sha256').update(passkey).digest('hex');
const hashMatch = html.match(/PASSHASH = '([a-f0-9]+)'/);
if (hashMatch && hash !== hashMatch[1]) {
  console.error('ERROR: Passkey does not match the stored SHA-256 hash.');
  console.error('Expected hash:', hashMatch[1]);
  console.error('Got hash:     ', hash);
  process.exit(1);
}

// Encrypt with AES-256-GCM using PBKDF2-derived key
const salt = crypto.randomBytes(16);
const iv = crypto.randomBytes(12);
const key = crypto.pbkdf2Sync(passkey, salt, 100000, 32, 'sha256');
const cipher = crypto.createCipheriv('aes-256-gcm', key, iv);

let encrypted = cipher.update(plaintext, 'utf8');
encrypted = Buffer.concat([encrypted, cipher.final()]);
const authTag = cipher.getAuthTag();

// Pack: salt(16) + iv(12) + authTag(16) + ciphertext
const packed = Buffer.concat([salt, iv, authTag, encrypted]);
const payload = packed.toString('base64');

console.log('Plaintext size:', plaintext.length, 'bytes');
console.log('Encrypted size:', payload.length, 'bytes (base64)');
console.log('Passkey hash verified: OK');

// Build the new resume.html with encrypted content
const beforeContent = html.substring(0, startIdx + startMarker.length);
const afterContent = html.substring(endIdx);

// Replace the plaintext with a hidden element containing the encrypted payload
const encryptedSection = `
<!-- Resume content is AES-256-GCM encrypted. Passkey required to decrypt. -->
<noscript><p style="text-align:center;padding:40px;color:#666;">JavaScript is required to view this document.</p></noscript>
<div id="encryptedPayload" style="display:none">${payload}</div>
`;

// Build new script that decrypts instead of just toggling CSS
// Note: innerHTML is used intentionally here to render the resume's own
// trusted HTML markup after decryption. The content is authored by the site
// owner and encrypted by this build script, not user-supplied input.
const newScript = `<script>
(function() {
  var gate = document.getElementById('accessGate');
  var content = document.getElementById('resumeContent');
  var input = document.getElementById('gateInput');
  var btn = document.getElementById('gateBtn');
  var error = document.getElementById('gateError');

  var focusableEls = gate.querySelectorAll('input, button, a[href]');
  var firstFocusable = focusableEls[0];
  var lastFocusable = focusableEls[focusableEls.length - 1];
  gate.addEventListener('keydown', function(e) {
    if (e.key !== 'Tab') return;
    if (e.shiftKey) {
      if (document.activeElement === firstFocusable) { e.preventDefault(); lastFocusable.focus(); }
    } else {
      if (document.activeElement === lastFocusable) { e.preventDefault(); firstFocusable.focus(); }
    }
  });

  var cached = sessionStorage.getItem('resumeDecrypted');
  if (cached) {
    content.innerHTML = cached; // trusted: own resume content decrypted from AES payload
    content.classList.add('unlocked');
    gate.classList.add('hidden');
    gate.setAttribute('aria-hidden', 'true');
  } else {
    input.focus();
  }

  async function deriveKey(passkey, salt) {
    var enc = new TextEncoder();
    var keyMaterial = await crypto.subtle.importKey('raw', enc.encode(passkey), 'PBKDF2', false, ['deriveKey']);
    return crypto.subtle.deriveKey(
      { name: 'PBKDF2', salt: salt, iterations: 100000, hash: 'SHA-256' },
      keyMaterial,
      { name: 'AES-GCM', length: 256 },
      false,
      ['decrypt']
    );
  }

  async function decryptResume(passkey) {
    var payloadEl = document.getElementById('encryptedPayload');
    if (!payloadEl) return false;
    var raw = Uint8Array.from(atob(payloadEl.textContent), function(c) { return c.charCodeAt(0); });
    var salt = raw.slice(0, 16);
    var iv = raw.slice(16, 28);
    var authTag = raw.slice(28, 44);
    var ciphertext = raw.slice(44);
    var combined = new Uint8Array(ciphertext.length + authTag.length);
    combined.set(ciphertext);
    combined.set(authTag, ciphertext.length);
    try {
      var key = await deriveKey(passkey, salt);
      var decrypted = await crypto.subtle.decrypt({ name: 'AES-GCM', iv: iv }, key, combined);
      return new TextDecoder().decode(decrypted);
    } catch (e) {
      return false;
    }
  }

  async function tryUnlock() {
    var val = input.value.trim();
    if (!val) { error.textContent = ''; return; }
    btn.textContent = '...';
    btn.disabled = true;
    var result = await decryptResume(val);
    if (result) {
      content.innerHTML = result; // trusted: own resume content decrypted from AES payload
      content.classList.add('unlocked');
      gate.classList.add('hidden');
      gate.setAttribute('aria-hidden', 'true');
      sessionStorage.setItem('resumeDecrypted', result);
    } else {
      error.textContent = 'Invalid access code.';
      input.value = '';
      input.focus();
    }
    btn.textContent = 'ENTER';
    btn.disabled = false;
  }

  btn.addEventListener('click', tryUnlock);
  input.addEventListener('keydown', function(e) {
    if (e.key === 'Enter') tryUnlock();
  });
})();

window.addEventListener('beforeprint', function(e) {
  if (!document.getElementById('resumeContent').classList.contains('unlocked')) {
    e.preventDefault();
    e.stopImmediatePropagation();
    return false;
  }
});
</script>`;

// Replace old script block (handle both \n and \r\n line endings)
let newHtml = beforeContent + encryptedSection + afterContent;

// Find the script block by searching for unique content markers
const osStart = newHtml.indexOf('<script>\r\n(function() {\r\n  // SHA-256') !== -1
  ? newHtml.indexOf('<script>\r\n(function() {\r\n  // SHA-256')
  : newHtml.indexOf('<script>\n(function() {\n  // SHA-256');

const osEndMarker = 'window.onbeforeprint = function() { return false; };';
const osEndIdx = newHtml.indexOf(osEndMarker);
const osEnd = osEndIdx !== -1
  ? newHtml.indexOf('</script>', osEndIdx) + '</script>'.length
  : -1;

if (osStart === -1 || osEnd === -1) {
  console.error('Could not find old script block to replace.');
  console.error('osStart:', osStart, 'osEnd:', osEnd);
  process.exit(1);
}

newHtml = newHtml.substring(0, osStart) + newScript + newHtml.substring(osEnd);

fs.writeFileSync(resumePath, newHtml, 'utf8');
console.log('\nresume.html has been encrypted and saved.');
console.log('The plaintext content is no longer in the file.');
console.log('View-source will show only encrypted base64 data.');
