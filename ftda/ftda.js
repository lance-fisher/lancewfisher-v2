/**
 * FTDA - Fisher Team Development Architecture
 * Passkey-gated document reader with copy deterrents
 *
 * NOTE: These are deterrents, not absolute prevention.
 * Client-side gating cannot prevent a determined user with developer tools.
 * For stronger protection, upgrade to server-side gating with signed URLs.
 *
 * SECURITY NOTE: innerHTML usage in this file renders content from the local
 * FTDA_CONFIG object (config.js), which is authored and controlled solely by
 * the site owner (Lance Fisher). No user-supplied or external content is rendered.
 * This is safe for this static site context.
 */

(function() {
  'use strict';

  // --- State ---
  var unlocked = false;
  var secureMode = false;
  var currentDocIndex = 0;
  var currentArtifact = null;
  var chunkObserver = null;

  // --- Utility: SHA-256 hash via WebCrypto ---
  function sha256(str) {
    var buf = new TextEncoder().encode(str);
    return crypto.subtle.digest('SHA-256', buf).then(function(hash) {
      return Array.from(new Uint8Array(hash)).map(function(b) {
        return b.toString(16).padStart(2, '0');
      }).join('');
    });
  }

  // --- Check session unlock ---
  function checkSession() {
    return sessionStorage.getItem('ftda_unlocked') === 'true';
  }
  function setSession() {
    sessionStorage.setItem('ftda_unlocked', 'true');
  }
  function clearSession() {
    sessionStorage.removeItem('ftda_unlocked');
  }

  // --- Build the FTDA featurette section ---
  // Now targets .ftda-flagship card inside the project grid
  function buildFeaturette(artifact) {
    var card = document.getElementById('ftda-section');
    if (!card) return;

    // Populate summary text from config
    var summaryEl = card.querySelector('.ftda-summary-text');
    if (summaryEl) summaryEl.textContent = artifact.hoverSummary;

    // Draw canvas thumbnail
    var canvas = document.getElementById('ftda-canvas');
    if (canvas && artifact.thumbnail.type === 'canvas') {
      drawFtdaThumbnail(canvas, artifact);
    } else if (artifact.thumbnail.type === 'image' && artifact.thumbnail.src) {
      var thumbWrap = canvas ? canvas.parentElement : null;
      if (thumbWrap) {
        var img = document.createElement('img');
        img.src = artifact.thumbnail.src;
        img.alt = artifact.title;
        img.loading = 'lazy';
        if (canvas) thumbWrap.removeChild(canvas);
        thumbWrap.appendChild(img);
      }
    }

    // Mobile tap toggle for detail expansion
    card.addEventListener('click', function(e) {
      if (window.innerWidth <= 768 && !e.target.closest('.ftda-cta')) {
        card.classList.toggle('expanded');
      }
    });

    // CTA click — open passkey or reader
    var cta = card.querySelector('.ftda-cta');
    if (cta) {
      cta.addEventListener('click', function(e) {
        e.preventDefault();
        e.stopPropagation();
        currentArtifact = artifact;
        if (checkSession()) {
          unlocked = true;
          openReader(artifact);
        } else {
          openPasskeyModal(artifact);
        }
      });
    }
  }

  // --- Premium canvas thumbnail ---
  function drawFtdaThumbnail(canvas, artifact) {
    var dpr = 2;
    var pw = canvas.parentElement.offsetWidth;
    var ph = canvas.parentElement.offsetHeight;
    var w = (pw > 10 ? pw : 500) * dpr;
    var h = (ph > 10 ? ph : 340) * dpr;
    canvas.width = w; canvas.height = h;
    canvas.style.width = '100%'; canvas.style.height = '100%';
    var ctx = canvas.getContext('2d');
    var s = dpr;

    // Deep dark background
    var bg = ctx.createLinearGradient(0, 0, w, h);
    bg.addColorStop(0, '#08080a'); bg.addColorStop(0.5, '#0c0b0e'); bg.addColorStop(1, '#0a0910');
    ctx.fillStyle = bg; ctx.fillRect(0, 0, w, h);

    // Subtle radial glow (gold)
    var glow = ctx.createRadialGradient(w*0.4, h*0.45, 0, w*0.4, h*0.45, w*0.5);
    glow.addColorStop(0, 'rgba(201, 168, 76, 0.06)');
    glow.addColorStop(1, 'transparent');
    ctx.fillStyle = glow; ctx.fillRect(0, 0, w, h);

    // Grid pattern
    ctx.strokeStyle = 'rgba(201, 168, 76, 0.015)'; ctx.lineWidth = 0.5*s;
    for (var gi = 0; gi < w; gi += 30*s) { ctx.beginPath(); ctx.moveTo(gi, 0); ctx.lineTo(gi, h); ctx.stroke(); }
    for (var gj = 0; gj < h; gj += 30*s) { ctx.beginPath(); ctx.moveTo(0, gj); ctx.lineTo(w, gj); ctx.stroke(); }

    // Large shield icon (center-left)
    var shX = w*0.28, shY = h*0.45, shS = 60*s;
    ctx.save();
    ctx.beginPath();
    ctx.moveTo(shX, shY - shS*0.65);
    ctx.lineTo(shX + shS*0.5, shY - shS*0.45);
    ctx.lineTo(shX + shS*0.5, shY + shS*0.1);
    ctx.quadraticCurveTo(shX, shY + shS*0.65, shX, shY + shS*0.65);
    ctx.quadraticCurveTo(shX, shY + shS*0.65, shX - shS*0.5, shY + shS*0.1);
    ctx.lineTo(shX - shS*0.5, shY - shS*0.45);
    ctx.closePath();
    var shGrad = ctx.createLinearGradient(shX, shY - shS*0.65, shX, shY + shS*0.65);
    shGrad.addColorStop(0, 'rgba(201, 168, 76, 0.06)');
    shGrad.addColorStop(1, 'rgba(201, 168, 76, 0.02)');
    ctx.fillStyle = shGrad; ctx.fill();
    ctx.strokeStyle = 'rgba(201, 168, 76, 0.15)'; ctx.lineWidth = 1.5*s; ctx.stroke();
    // Inner shield
    ctx.beginPath();
    ctx.moveTo(shX, shY - shS*0.45);
    ctx.lineTo(shX + shS*0.32, shY - shS*0.3);
    ctx.lineTo(shX + shS*0.32, shY + shS*0.05);
    ctx.quadraticCurveTo(shX, shY + shS*0.45, shX, shY + shS*0.45);
    ctx.quadraticCurveTo(shX, shY + shS*0.45, shX - shS*0.32, shY + shS*0.05);
    ctx.lineTo(shX - shS*0.32, shY - shS*0.3);
    ctx.closePath();
    ctx.strokeStyle = 'rgba(201, 168, 76, 0.08)'; ctx.lineWidth = 0.8*s; ctx.stroke();
    ctx.restore();

    // Star in shield center
    drawStar(ctx, shX, shY - shS*0.05, 8*s, 4*s, 5, 'rgba(201, 168, 76, 0.2)');

    // Right side: document lines mockup
    var docX = w*0.55, docY = h*0.15, docW = w*0.36, docH = h*0.7;
    ctx.fillStyle = 'rgba(201, 168, 76, 0.02)'; ctx.fillRect(docX, docY, docW, docH);
    ctx.strokeStyle = 'rgba(201, 168, 76, 0.06)'; ctx.lineWidth = 0.5*s;
    ctx.strokeRect(docX, docY, docW, docH);

    // Doc header
    ctx.fillStyle = 'rgba(201, 168, 76, 0.04)'; ctx.fillRect(docX, docY, docW, 20*s);
    ctx.fillStyle = 'rgba(201, 168, 76, 0.3)'; ctx.font = '200 '+(5*s)+'px Inter, sans-serif';
    ctx.fillText('ARCHITECTURE PLAYBOOK', docX+8*s, docY+13*s);

    // Document line mockups
    var lineY = docY + 30*s;
    var lineWidths = [0.85, 0.65, 0.9, 0.4, 0, 0.7, 0.82, 0.55, 0.9, 0.3, 0, 0.6, 0.78, 0.88, 0.45, 0, 0.72, 0.83, 0.55, 0.9];
    for (var li = 0; li < lineWidths.length && lineY < docY+docH-10*s; li++) {
      if (lineWidths[li] === 0) { lineY += 10*s; continue; }
      var lw = lineWidths[li] * (docW - 16*s);
      var isHeading = (li === 0 || li === 5 || li === 11 || li === 16);
      ctx.fillStyle = isHeading ? 'rgba(240, 235, 224, 0.12)' : 'rgba(240, 235, 224, 0.04)';
      ctx.fillRect(docX+8*s, lineY, lw, isHeading ? 3*s : 2*s);
      lineY += isHeading ? 12*s : 8*s;
    }

    // Lock icon badge
    ctx.fillStyle = 'rgba(201, 168, 76, 0.1)';
    ctx.beginPath(); ctx.arc(docX+docW-14*s, docY+docH-14*s, 10*s, 0, Math.PI*2); ctx.fill();
    ctx.strokeStyle = 'rgba(201, 168, 76, 0.2)'; ctx.lineWidth = 0.8*s; ctx.stroke();
    ctx.fillStyle = 'rgba(201, 168, 76, 0.4)'; ctx.font = '500 '+(8*s)+'px Inter, sans-serif';
    ctx.textAlign = 'center'; ctx.fillText('\u{1F512}', docX+docW-14*s, docY+docH-10*s);
    ctx.textAlign = 'left';

    // Title text overlay
    ctx.fillStyle = 'rgba(240, 235, 224, 0.45)'; ctx.font = '300 '+(14*s)+'px Cormorant Garamond, serif';
    ctx.fillText('Fisher Team', w*0.06, h*0.14);
    ctx.fillText('Development', w*0.06, h*0.21);
    ctx.fillStyle = 'rgba(201, 168, 76, 0.35)';
    ctx.fillText('Architecture', w*0.06, h*0.28);

    // Decorative rule
    ctx.fillStyle = 'rgba(201, 168, 76, 0.12)';
    ctx.fillRect(w*0.06, h*0.31, 50*s, 0.5*s);

    // Subtitle
    ctx.fillStyle = 'rgba(240, 235, 224, 0.15)'; ctx.font = '200 '+(6*s)+'px Inter, sans-serif';
    ctx.fillText('LEADERSHIP FRAMEWORK', w*0.06, h*0.35);

    // Bottom stats bar
    var bStatY = h*0.88;
    ctx.fillStyle = 'rgba(201, 168, 76, 0.03)'; ctx.fillRect(0, bStatY-4*s, w, h-bStatY+4*s);
    ctx.strokeStyle = 'rgba(201, 168, 76, 0.04)'; ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, bStatY-4*s); ctx.lineTo(w, bStatY-4*s); ctx.stroke();

    var bStats = [{v:'15+', l:'Years Leading'}, {v:'6', l:'Pillars'}, {v:'2', l:'Documents'}, {v:'\u{1F512}', l:'Protected'}];
    var bsW = w / bStats.length;
    for (var bsi = 0; bsi < bStats.length; bsi++) {
      var bsx = bsi * bsW + bsW/2;
      ctx.fillStyle = 'rgba(201, 168, 76, 0.5)'; ctx.font = '300 '+(10*s)+'px Cormorant Garamond, serif';
      ctx.textAlign = 'center'; ctx.fillText(bStats[bsi].v, bsx, bStatY+8*s);
      ctx.fillStyle = 'rgba(240, 235, 224, 0.12)'; ctx.font = '200 '+(4.5*s)+'px Inter, sans-serif';
      ctx.fillText(bStats[bsi].l, bsx, bStatY+16*s);
    }
    ctx.textAlign = 'left';

    // Corner brackets
    var c = 14*s;
    ctx.strokeStyle = 'rgba(201, 168, 76, 0.12)'; ctx.lineWidth = 1*s;
    ctx.beginPath(); ctx.moveTo(1, c); ctx.lineTo(1, 1); ctx.lineTo(c, 1); ctx.stroke();
    ctx.beginPath(); ctx.moveTo(w-c, 1); ctx.lineTo(w-1, 1); ctx.lineTo(w-1, c); ctx.stroke();
    ctx.beginPath(); ctx.moveTo(1, h-c); ctx.lineTo(1, h-1); ctx.lineTo(c, h-1); ctx.stroke();
    ctx.beginPath(); ctx.moveTo(w-c, h-1); ctx.lineTo(w-1, h-1); ctx.lineTo(w-1, h-c); ctx.stroke();
  }

  function drawStar(ctx, cx, cy, outerR, innerR, points, color) {
    ctx.beginPath();
    for (var i = 0; i < points * 2; i++) {
      var r = i % 2 === 0 ? outerR : innerR;
      var angle = (Math.PI * i / points) - Math.PI / 2;
      if (i === 0) ctx.moveTo(cx + r * Math.cos(angle), cy + r * Math.sin(angle));
      else ctx.lineTo(cx + r * Math.cos(angle), cy + r * Math.sin(angle));
    }
    ctx.closePath();
    ctx.fillStyle = color; ctx.fill();
  }


  // --- Passkey Modal ---
  function openPasskeyModal() {
    var overlay = document.getElementById('ftda-passkey-overlay');
    if (!overlay) return;
    overlay.classList.add('active');
    overlay.setAttribute('aria-hidden', 'false');
    var input = overlay.querySelector('.ftda-passkey-input');
    if (input) {
      input.value = '';
      setTimeout(function() { input.focus(); }, 100);
    }
    var err = overlay.querySelector('.ftda-passkey-error');
    if (err) err.classList.remove('visible');
  }

  function closePasskeyModal() {
    var overlay = document.getElementById('ftda-passkey-overlay');
    if (overlay) {
      overlay.classList.remove('active');
      overlay.setAttribute('aria-hidden', 'true');
    }
  }

  function submitPasskey() {
    var input = document.querySelector('.ftda-passkey-input');
    var err = document.querySelector('.ftda-passkey-error');
    if (!input) return;

    var val = input.value.trim();
    if (!val) {
      if (err) { err.textContent = 'Please enter a passkey'; err.classList.add('visible'); }
      return;
    }

    sha256(val).then(function(hash) {
      if (hash === FTDA_CONFIG.passkeySha256Hex) {
        unlocked = true;
        setSession();
        closePasskeyModal();
        if (currentArtifact) openReader(currentArtifact);
      } else {
        if (err) { err.textContent = 'Invalid passkey. Please try again.'; err.classList.add('visible'); }
        input.value = '';
        input.focus();
      }
    });
  }


  // --- Document Reader ---
  function openReader(artifact) {
    var overlay = document.getElementById('ftda-reader-overlay');
    if (!overlay) return;

    currentDocIndex = 0;

    // Build TOC - using owner-controlled content from FTDA_CONFIG (safe)
    var toc = overlay.querySelector('.ftda-reader-toc');
    if (toc) {
      // Clear existing content
      while (toc.firstChild) toc.removeChild(toc.firstChild);

      var heading = document.createElement('div');
      heading.className = 'ftda-toc-heading';
      heading.textContent = 'Contents';
      toc.appendChild(heading);

      artifact.documents.forEach(function(doc, idx) {
        var btn = document.createElement('button');
        btn.className = 'ftda-toc-item' + (idx === 0 ? ' active' : '');
        btn.setAttribute('data-doc-idx', idx);
        btn.textContent = doc.label;
        btn.addEventListener('click', function() {
          switchDocument(artifact, idx);
          toc.querySelectorAll('.ftda-toc-item').forEach(function(b) { b.classList.remove('active'); });
          btn.classList.add('active');
        });
        toc.appendChild(btn);
      });
    }

    // Build watermark
    buildWatermark(artifact.watermarkText);

    // Render first document
    renderDocument(artifact, 0);

    // Show overlay
    overlay.classList.add('active');
    overlay.setAttribute('aria-hidden', 'false');
    document.body.style.overflow = 'hidden';
  }

  function closeReader() {
    var overlay = document.getElementById('ftda-reader-overlay');
    if (overlay) {
      overlay.classList.remove('active');
      overlay.setAttribute('aria-hidden', 'true');
    }
    var wm = document.querySelector('.ftda-watermark');
    if (wm) wm.style.display = 'none';
    document.body.style.overflow = '';
    if (chunkObserver) { chunkObserver.disconnect(); chunkObserver = null; }
  }

  function lockReader() {
    unlocked = false;
    clearSession();
    closeReader();
  }

  function switchDocument(artifact, idx) {
    currentDocIndex = idx;
    renderDocument(artifact, idx);
  }

  // --- Chunked progressive document rendering ---
  // Content comes from FTDA_CONFIG (owner-controlled, local config file)
  function renderDocument(artifact, docIdx) {
    var contentArea = document.querySelector('.ftda-reader-content');
    if (!contentArea || !artifact.documents[docIdx]) return;

    var doc = artifact.documents[docIdx];

    // Clear existing content
    while (contentArea.firstChild) contentArea.removeChild(contentArea.firstChild);

    var wrapper = document.createElement('div');
    wrapper.className = 'ftda-doc-content';

    // Copyright notice at top
    var topCopy = document.createElement('div');
    topCopy.className = 'ftda-copyright';
    topCopy.textContent = artifact.copyrightNotice;
    wrapper.appendChild(topCopy);

    // Parse content into chunks
    var rawContent = doc.content;
    if (doc.contentType === 'text') {
      rawContent = rawContent.split('\n\n').map(function(p) {
        var span = document.createElement('p');
        span.textContent = p;
        return span.outerHTML;
      }).join('');
    }

    // Split into chunks by block elements
    var chunks = splitIntoChunks(rawContent);

    // Render chunks as DOM elements
    // Note: content is from local FTDA_CONFIG authored by the site owner
    chunks.forEach(function(chunk, idx) {
      var chunkDiv = document.createElement('div');
      chunkDiv.className = 'ftda-chunk';
      chunkDiv.setAttribute('data-chunk', idx);
      // Content originates from the owner's config.js, not from external/user input
      chunkDiv.innerHTML = chunk;
      wrapper.appendChild(chunkDiv);
    });

    // Signature
    var sig = document.createElement('div');
    sig.className = 'ftda-signature';
    sig.textContent = 'Lance Fisher, ' + new Date().getFullYear();
    wrapper.appendChild(sig);

    // Copyright at bottom
    var botCopy = document.createElement('div');
    botCopy.className = 'ftda-copyright';
    botCopy.style.marginTop = '32px';
    botCopy.textContent = artifact.copyrightNotice;
    wrapper.appendChild(botCopy);

    contentArea.appendChild(wrapper);

    // Setup progressive reveal
    setupChunkReveal(contentArea);

    // Scroll to top
    contentArea.scrollTop = 0;
  }

  function splitIntoChunks(html) {
    var chunks = [];
    var temp = document.createElement('div');
    // Content is from local config.js (owner-authored)
    temp.innerHTML = html;
    var children = temp.children;
    var currentChunk = '';
    var charCount = 0;
    var targetChunkSize = 800;

    for (var i = 0; i < children.length; i++) {
      var child = children[i].outerHTML;
      currentChunk += child;
      charCount += children[i].textContent.length;

      if (charCount >= targetChunkSize || i === children.length - 1) {
        chunks.push(currentChunk);
        currentChunk = '';
        charCount = 0;
      }
    }

    if (chunks.length === 0 && html.trim()) {
      chunks.push(html);
    }

    return chunks;
  }

  function setupChunkReveal(container) {
    if (chunkObserver) chunkObserver.disconnect();

    var chunks = container.querySelectorAll('.ftda-chunk');

    chunkObserver = new IntersectionObserver(function(entries) {
      entries.forEach(function(entry) {
        if (entry.isIntersecting) {
          entry.target.classList.add('revealed');
        }
      });
    }, { root: container, threshold: 0.1 });

    chunks.forEach(function(chunk) {
      chunkObserver.observe(chunk);
    });
  }


  // --- Watermark ---
  function buildWatermark(text) {
    var wm = document.querySelector('.ftda-watermark');
    if (!wm) return;

    var inner = wm.querySelector('.ftda-watermark-inner');
    if (!inner) return;

    // Clear and rebuild
    while (inner.firstChild) inner.removeChild(inner.firstChild);
    var count = 80;
    for (var i = 0; i < count; i++) {
      var span = document.createElement('span');
      span.className = 'ftda-watermark-text';
      span.textContent = text;
      inner.appendChild(span);
    }
    wm.style.display = 'block';
  }


  // --- Copy Protection Deterrents ---
  function setupCopyProtection() {
    var reader = document.getElementById('ftda-reader-overlay');
    if (!reader) return;

    // Block right-click in reader
    reader.addEventListener('contextmenu', function(e) {
      e.preventDefault();
      return false;
    });

    // Block copy shortcuts in reader
    reader.addEventListener('keydown', function(e) {
      var key = e.key ? e.key.toLowerCase() : '';
      var ctrl = e.ctrlKey || e.metaKey;

      if (ctrl && (key === 'c' || key === 'a' || key === 's' || key === 'p' || key === 'u')) {
        e.preventDefault();
        return false;
      }
    });

    // Secure mode toggle
    var secureBtn = reader.querySelector('[data-ftda-secure]');
    if (secureBtn) {
      secureBtn.addEventListener('click', function() {
        secureMode = !secureMode;
        reader.classList.toggle('ftda-secure-mode', secureMode);
        secureBtn.classList.toggle('active', secureMode);
        secureBtn.textContent = secureMode ? 'Secure: On' : 'Secure: Off';
      });
    }

    // Watermark toggle
    var wmBtn = reader.querySelector('[data-ftda-watermark]');
    if (wmBtn) {
      wmBtn.addEventListener('click', function() {
        var wm = document.querySelector('.ftda-watermark');
        if (!wm) return;
        var visible = wm.style.opacity !== '0';
        wm.style.opacity = visible ? '0' : '';
        wmBtn.classList.toggle('active', !visible);
        wmBtn.textContent = visible ? 'Watermark: Off' : 'Watermark: On';
      });
    }
  }


  // --- Init ---
  function init() {
    if (typeof FTDA_CONFIG === 'undefined' || !FTDA_CONFIG.artifacts || !FTDA_CONFIG.artifacts.length) return;

    var artifact = FTDA_CONFIG.artifacts[0];
    currentArtifact = artifact;

    buildFeaturette(artifact);
    setupCopyProtection();

    // Passkey modal events
    var passkeyOverlay = document.getElementById('ftda-passkey-overlay');
    if (passkeyOverlay) {
      passkeyOverlay.querySelector('.ftda-passkey-close').addEventListener('click', closePasskeyModal);
      passkeyOverlay.querySelector('.ftda-passkey-submit').addEventListener('click', submitPasskey);
      var input = passkeyOverlay.querySelector('.ftda-passkey-input');
      if (input) {
        input.addEventListener('keydown', function(e) {
          if (e.key === 'Enter') submitPasskey();
        });
      }
      passkeyOverlay.addEventListener('click', function(e) {
        if (e.target === passkeyOverlay) closePasskeyModal();
      });
    }

    // Reader events
    var readerOverlay = document.getElementById('ftda-reader-overlay');
    if (readerOverlay) {
      readerOverlay.querySelector('.ftda-reader-close').addEventListener('click', closeReader);
      var lockBtn = readerOverlay.querySelector('[data-ftda-lock]');
      if (lockBtn) lockBtn.addEventListener('click', lockReader);
    }

    // Escape key
    document.addEventListener('keydown', function(e) {
      if (e.key === 'Escape') {
        if (document.querySelector('.ftda-reader-overlay.active')) closeReader();
        else if (document.querySelector('.ftda-passkey-overlay.active')) closePasskeyModal();
      }
    });
  }

  // Run on DOM ready
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }

})();
