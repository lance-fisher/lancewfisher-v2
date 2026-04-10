(function() {
  'use strict';

  // ===== PORTFOLIO EXPAND/COLLAPSE =====
  function togglePortfolio() {
    var btn = document.getElementById('portfolioExpandBtn');
    var remaining = document.getElementById('portfolioRemaining');
    var arrow = btn.querySelector('.expand-arrow');
    var isExpanded = remaining.classList.contains('expanded');
    if (isExpanded) {
      remaining.classList.remove('expanded');
      btn.classList.remove('expanded');
      btn.firstChild.textContent = 'View Full Portfolio (9 more projects) ';
      btn.scrollIntoView({ behavior: 'smooth', block: 'center' });
    } else {
      remaining.classList.add('expanded');
      btn.classList.add('expanded');
      btn.firstChild.textContent = 'Collapse Portfolio ';
    }
  }
  window.togglePortfolio = togglePortfolio;

  // ===== PROJECT CATEGORY FILTERS =====
  (function() {
    var filterBtns = document.querySelectorAll('.pf-btn');
    var allProjectCards = document.querySelectorAll('.project-card[data-tags]');
    var expandBtn = document.getElementById('portfolioExpandBtn');
    var remaining = document.getElementById('portfolioRemaining');

    filterBtns.forEach(function(btn) {
      btn.addEventListener('click', function() {
        filterBtns.forEach(function(b) { b.classList.remove('active'); });
        btn.classList.add('active');
        var filter = btn.getAttribute('data-filter');

        // When filtering, expand the hidden section so all matches are visible
        if (filter !== 'all' && remaining && !remaining.classList.contains('expanded')) {
          remaining.classList.add('expanded');
          if (expandBtn) {
            expandBtn.classList.add('expanded');
            expandBtn.firstChild.textContent = 'Collapse Portfolio ';
          }
        }

        allProjectCards.forEach(function(card) {
          if (filter === 'all' || card.getAttribute('data-tags') === filter) {
            card.style.display = '';
          } else {
            card.style.display = 'none';
          }
        });
      });
    });
  })();

  // ===== PHONE NUMBER RENDERING (anti-scrape) =====
  document.querySelectorAll('[data-p]').forEach(function(el) {
    try {
      var decoded = atob(el.getAttribute('data-p'));
      var formatted = '(' + decoded.substring(0,3) + ') ' + decoded.substring(4);
      el.textContent = formatted;
      var parent = el.closest('.contact-row');
      if (parent) {
        var raw = decoded.replace(/-/g, '');
        parent.onclick = function() { window.location.href = 'tel:+1' + raw; };
      }
    } catch(e) {}
  });

  // ===== BOOKING FORM SUBMISSION =====
  (function() {
    var form = document.getElementById('bookingForm');
    var wrap = document.getElementById('bookingFormWrap');
    var success = document.getElementById('bookingSuccess');
    var submitBtn = document.getElementById('bookingSubmit');
    if (!form) return;
    form.addEventListener('submit', function(e) {
      e.preventDefault();
      submitBtn.disabled = true;
      submitBtn.textContent = 'Sending...';
      var formData = new FormData(form);
      fetch(form.action, {
        method: 'POST',
        body: formData,
        headers: { 'Accept': 'application/json' }
      }).then(function(response) {
        return response.json().then(function(data) {
          if (response.ok && data.ok) {
            form.style.display = 'none';
            success.classList.add('visible');
          } else {
            var msg = data.error || (data.errors ? data.errors.join(' ') : 'Something went wrong.');
            submitBtn.disabled = false;
            submitBtn.textContent = 'Send Request';
            alert(msg + ' You can also email lance@lancewfisher.com directly.');
          }
        });
      }).catch(function() {
        submitBtn.disabled = false;
        submitBtn.textContent = 'Send Request';
        alert('Something went wrong. Please email lance@lancewfisher.com directly.');
      });
    });
  })();

  // ===== THEME TOGGLE =====
  var toggleBtn = document.getElementById('themeToggle');
  function getTheme() {
    var saved = localStorage.getItem('lf-theme');
    if (saved) return saved;
    return window.matchMedia('(prefers-color-scheme: light)').matches ? 'light' : 'dark';
  }
  function setTheme(t) {
    if (t === 'light') {
      document.documentElement.setAttribute('data-theme', 'light');
    } else {
      document.documentElement.removeAttribute('data-theme');
    }
    localStorage.setItem('lf-theme', t);
  }
  if (toggleBtn) {
    toggleBtn.addEventListener('click', function() {
      var current = document.documentElement.getAttribute('data-theme') === 'light' ? 'light' : 'dark';
      setTheme(current === 'dark' ? 'light' : 'dark');
      // Re-render all canvases for new theme
      if (typeof initProjectCanvases === 'function') initProjectCanvases();
      if (typeof initFtdaCanvas === 'function') initFtdaCanvas();
    });
  }
  // Listen for system preference changes
  window.matchMedia('(prefers-color-scheme: light)').addEventListener('change', function(e) {
    if (!localStorage.getItem('lf-theme')) {
      setTheme(e.matches ? 'light' : 'dark');
    }
  });

  // ===== SCROLL REVEAL =====
  var sections = document.querySelectorAll('[data-reveal]');
  var isMobile = window.innerWidth <= 768;

  // Use more aggressive threshold on mobile (1% vs 8%)
  var observer = new IntersectionObserver(function(entries) {
    entries.forEach(function(entry) {
      if (entry.isIntersecting) entry.target.classList.add('visible');
    });
  }, { threshold: isMobile ? 0.01 : 0.08, rootMargin: isMobile ? '0px 0px 0px 0px' : '0px 0px -60px 0px' });
  sections.forEach(function(s) { observer.observe(s); });

  // Mobile fallback: also check on scroll in case IntersectionObserver doesn't fire
  if (isMobile) {
    function mobileRevealCheck() {
      var vh = window.innerHeight;
      sections.forEach(function(s) {
        var rect = s.getBoundingClientRect();
        // If section top is within 1.2x viewport height, reveal it
        if (rect.top < vh * 1.2) {
          s.classList.add('visible');
        }
      });
    }
    window.addEventListener('scroll', mobileRevealCheck, { passive: true });
    // Run once on load after a small delay
    setTimeout(mobileRevealCheck, 500);
    setTimeout(mobileRevealCheck, 1500);
  }

  // ===== SCROLL INDICATOR =====
  var scrollInd = document.getElementById('scrollIndicator');
  window.addEventListener('scroll', function() {
    if (window.scrollY > 120) scrollInd.classList.add('hidden');
    else scrollInd.classList.remove('hidden');
  }, { passive: true });

  // ===== WATERMARK PARALLAX =====
  var ship = document.querySelector('.ship-watermark');
  var statue = document.querySelector('.statue-watermark');
  window.addEventListener('scroll', function() {
    var y = window.scrollY;
    if (ship) ship.style.transform = 'translate(-50%, calc(-50% + ' + (y * 0.05) + 'px))';
    var isLight = document.documentElement.getAttribute('data-theme') === 'light';
    if (statue && !isLight) {
      // Dark mode only — light mode statue stays perfectly fixed via CSS
      statue.style.transform = 'translate(-50%, calc(-50% + ' + (y * 0.03) + 'px))';
    }
    // Scroll progress line (light mode only)
    if (isLight) {
      var maxScroll = document.documentElement.scrollHeight - window.innerHeight;
      var pct = maxScroll > 0 ? Math.min(y / maxScroll, 1) : 0;
      document.documentElement.style.setProperty('--sp', pct);
    }
  }, { passive: true });

  // ===== NAV DOTS =====
  var dots = document.querySelectorAll('.nav-dot');
  var sectionIds = [];
  dots.forEach(function(d) { sectionIds.push(d.getAttribute('data-target')); });
  dots.forEach(function(dot) {
    dot.addEventListener('click', function() {
      var target = document.getElementById(dot.getAttribute('data-target'));
      if (target) target.scrollIntoView({ behavior: 'smooth' });
    });
  });
  function updateDots() {
    var scrollY = window.scrollY, vh = window.innerHeight, active = 0;
    for (var i = sectionIds.length - 1; i >= 0; i--) {
      var el = document.getElementById(sectionIds[i]);
      if (el && el.getBoundingClientRect().top < vh * 0.5) { active = i; break; }
    }
    if (scrollY < 200) active = 0;
    dots.forEach(function(d, idx) { d.classList.toggle('active', idx === active); });
  }
  window.addEventListener('scroll', updateDots, { passive: true });
  updateDots();

  // ===== MOBILE TAP-TO-EXPAND PROJECT CARDS =====
  if (isMobile) {
    var projectCards = document.querySelectorAll('.project-card');
    projectCards.forEach(function(card) {
      // Skip cards with onclick (they navigate somewhere)
      if (card.getAttribute('onclick')) return;
      card.style.cursor = 'pointer';
      card.addEventListener('click', function(e) {
        // Don't interfere with links inside
        if (e.target.tagName === 'A') return;
        // Close other cards
        projectCards.forEach(function(c) {
          if (c !== card) c.classList.remove('expanded');
        });
        card.classList.toggle('expanded');
      });
    });
  }

  // ===== FISHER MORPH -MATRIX DECODE EFFECT =====
  var morphI = document.getElementById('morph-i');
  var morphE = document.getElementById('morph-e');
  if (morphI && morphE) {
    var charI = morphI.querySelector('.morph-char');
    var charE = morphE.querySelector('.morph-char');
    var morphOrigI = charI.textContent;
    var morphOrigE = charE.textContent;
    var morphRunning = false;
    var glyphPool = '0123456789!@#$%&*?><|/=+-~^';

    function randomGlyph() {
      return glyphPool.charAt(Math.floor(Math.random() * glyphPool.length));
    }

    function clearMorphState(el) {
      el.classList.remove('scanning', 'decoded', 'reverting');
    }

    // Pre-schedule all character changes as absolute timeouts from t=0
    // This avoids chaining/recursion and works under timer throttling
    function scheduleScramble(charEl, wrapEl, startMs, count, gap, finalChar, startClass, finalClass) {
      // Add scanning/reverting class at start
      setTimeout(function() {
        clearMorphState(wrapEl);
        if (startClass) wrapEl.classList.add(startClass);
      }, startMs);

      // Schedule each random glyph
      for (var i = 0; i < count; i++) {
        (function(idx) {
          setTimeout(function() {
            charEl.textContent = randomGlyph();
          }, startMs + (idx * gap));
        })(i);
      }

      // Land on final character
      var endMs = startMs + (count * gap);
      setTimeout(function() {
        charEl.textContent = finalChar;
        clearMorphState(wrapEl);
        if (finalClass) wrapEl.classList.add(finalClass);
      }, endMs);

      return endMs;
    }

    function runMorphCycle() {
      if (morphRunning) return;
      morphRunning = true;

      // Phase 1: Decode I → 1 (7 glyphs at 55ms)
      scheduleScramble(charI, morphI, 0, 7, 55, '1', 'scanning', 'decoded');

      // Phase 1b: Decode E → 3 (staggered 300ms)
      scheduleScramble(charE, morphE, 300, 7, 55, '3', 'scanning', 'decoded');

      // Phase 2: Revert I (after 2.4s hold)
      scheduleScramble(charI, morphI, 2400, 4, 50, morphOrigI, 'reverting', '');

      // Phase 2b: Revert E (staggered)
      scheduleScramble(charE, morphE, 2650, 4, 50, morphOrigE, 'reverting', '');

      // Safety net: hard reset at 3.5s
      setTimeout(function() {
        clearMorphState(morphI);
        clearMorphState(morphE);
        charI.textContent = morphOrigI;
        charE.textContent = morphOrigE;
        morphRunning = false;
      }, 3500);
    }

    // First morph after 4-6s, then every 10-15s
    setTimeout(function() {
      runMorphCycle();
      setInterval(runMorphCycle, 10000 + Math.random() * 5000);
    }, 4000 + Math.random() * 2000);
  }

  // ===== PORTRAIT CAROUSEL - GLITCH TRANSITION =====
  var carouselFrame = document.getElementById('portrait-carousel');
  if (carouselFrame) {
    var carouselImgs = carouselFrame.querySelectorAll('.portrait-secondary');
    var carouselScanline = carouselFrame.querySelector('.portrait-scanline');
    var cIdx = 0;
    var cBusy = false;

    function resetGlitch(img) {
      img.classList.remove('glitch-in', 'glitch-hold', 'glitch-out');
      img.removeAttribute('style');
    }

    function pulseScanline() {
      if (!carouselScanline) return;
      carouselScanline.classList.remove('active');
      void carouselScanline.offsetWidth;
      carouselScanline.classList.add('active');
    }

    function glitchCycle() {
      if (cBusy || carouselImgs.length === 0) return;
      cBusy = true;

      var img = carouselImgs[cIdx];
      cIdx = (cIdx + 1) % carouselImgs.length;

      // Reset any leftover state
      resetGlitch(img);

      // Phase 1: Scanline flash + glitch reveal (0 - 500ms)
      pulseScanline();
      img.classList.add('glitch-in');

      // Phase 2: Hold at full opacity (500ms - 1500ms)
      setTimeout(function() {
        img.classList.remove('glitch-in');
        img.classList.add('glitch-hold');
      }, 500);

      // Phase 3: Glitch dismiss (1500ms - 1900ms)
      setTimeout(function() {
        img.classList.remove('glitch-hold');
        img.classList.add('glitch-out');
        pulseScanline();
      }, 1500);

      // Phase 4: Cleanup (2000ms)
      setTimeout(function() {
        resetGlitch(img);
        cBusy = false;
      }, 2000);
    }

    // First glitch after 4-5s, then every 6-9s
    setTimeout(function() {
      glitchCycle();
      setInterval(glitchCycle, 6000 + Math.random() * 3000);
    }, 4000 + Math.random() * 1000);
  }

  // ===== IMAGE PRELOADER =====
  var imgCache = {};
  function loadImg(src, cb) {
    if (imgCache[src]) { cb(imgCache[src]); return; }
    var img = new Image();
    img.crossOrigin = 'anonymous';
    img.onload = function() { imgCache[src] = img; cb(img); };
    img.onerror = function() { cb(null); };
    img.src = src;
  }

  // ===== HELPERS =====
  function roundRect(ctx, x, y, w, h, r) {
    ctx.beginPath();
    ctx.moveTo(x + r, y);
    ctx.lineTo(x + w - r, y);
    ctx.quadraticCurveTo(x + w, y, x + w, y + r);
    ctx.lineTo(x + w, y + h - r);
    ctx.quadraticCurveTo(x + w, y + h, x + w - r, y + h);
    ctx.lineTo(x + r, y + h);
    ctx.quadraticCurveTo(x, y + h, x, y + h - r);
    ctx.lineTo(x, y + r);
    ctx.quadraticCurveTo(x, y, x + r, y);
    ctx.closePath();
  }

  function drawCorners(ctx, w, h, c, color) {
    ctx.strokeStyle = color;
    ctx.lineWidth = 2;
    ctx.beginPath(); ctx.moveTo(1, c); ctx.lineTo(1, 1); ctx.lineTo(c, 1); ctx.stroke();
    ctx.beginPath(); ctx.moveTo(w-c, 1); ctx.lineTo(w-1, 1); ctx.lineTo(w-1, c); ctx.stroke();
    ctx.beginPath(); ctx.moveTo(1, h-c); ctx.lineTo(1, h-1); ctx.lineTo(c, h-1); ctx.stroke();
    ctx.beginPath(); ctx.moveTo(w-c, h-1); ctx.lineTo(w-1, h-1); ctx.lineTo(w-1, h-c); ctx.stroke();
  }

  // ===== PROJECT THUMBNAIL RENDERERS =====
  // Each project gets a unique, detailed mockup

  function setupCanvas(canvas) {
    var dpr = 2;
    var pw = canvas.parentElement.offsetWidth;
    var ph = canvas.parentElement.offsetHeight;
    var w = (pw > 10 ? pw : 400) * dpr;
    var h = (ph > 10 ? ph : 250) * dpr;
    canvas.width = w; canvas.height = h;
    canvas.style.width = '100%'; canvas.style.height = '100%';
    return { ctx: canvas.getContext('2d'), w: w, h: h, s: dpr };
  }

  function isLightTheme() {
    return document.documentElement.getAttribute('data-theme') === 'light';
  }
  function pal(r, g, b) {
    var light = isLightTheme();
    return {
      bg1: light ? '#e8e9eb' : '#0d0d0e',
      bg2: light ? '#dddee0' : '#080809',
      glowAlpha: light ? 0.08 : 0.05,
      panelFill: light ? 'rgba('+r+','+g+','+b+',0.06)' : 'rgba('+r+','+g+','+b+',0.025)',
      panelStroke: light ? 'rgba('+r+','+g+','+b+',0.12)' : 'rgba('+r+','+g+','+b+',0.05)',
      panelHeavy: light ? 'rgba('+r+','+g+','+b+',0.1)' : 'rgba('+r+','+g+','+b+',0.06)',
      textPrimary: light ? 'rgba(26,26,28,0.88)' : 'rgba(240,235,224,0.75)',
      textSecondary: light ? 'rgba(26,26,28,0.65)' : 'rgba(240,235,224,0.45)',
      textDim: light ? 'rgba(26,26,28,0.4)' : 'rgba(240,235,224,0.25)',
      textBright: light ? 'rgba(26,26,28,0.95)' : 'rgba(240,235,224,0.85)',
      accentLabel: light ? 'rgba('+r+','+g+','+b+',0.55)' : 'rgba('+r+','+g+','+b+',0.35)',
      accentDim: light ? 'rgba('+r+','+g+','+b+',0.3)' : 'rgba('+r+','+g+','+b+',0.15)',
      accentGlow: light ? 'rgba('+r+','+g+','+b+',0.12)' : 'rgba('+r+','+g+','+b+',0.06)',
      accentMed: light ? 'rgba('+r+','+g+','+b+',0.5)' : 'rgba('+r+','+g+','+b+',0.3)',
      accentStrong: light ? 'rgba('+r+','+g+','+b+',0.7)' : 'rgba('+r+','+g+','+b+',0.5)',
      cornerColor: light ? 'rgba('+r+','+g+','+b+',0.2)' : 'rgba('+r+','+g+','+b+',0.15)',
      deviceBg: light ? '#f5f5f7' : '#1a1a1c',
      deviceBorder: light ? 'rgba(26,26,28,0.1)' : 'rgba(240,235,224,0.08)',
      gridLine: light ? 'rgba('+r+','+g+','+b+',0.06)' : 'rgba('+r+','+g+','+b+',0.02)',
      gridLineMed: light ? 'rgba('+r+','+g+','+b+',0.1)' : 'rgba('+r+','+g+','+b+',0.03)',
      shadowColor: light ? 'rgba(26,26,28,0.15)' : 'rgba(0,0,0,0.4)',
      r: r, g: g, b: b,
      light: light
    };
  }

  // Theme-aware text color: swaps warm-white to dark text
  function tClr(p, a) {
    return p.light ? 'rgba(26,26,28,' + Math.min(1, a * 1.15).toFixed(3) + ')' : 'rgba(240,235,224,' + a + ')';
  }
  // Theme-aware accent color: boosts alpha on light backgrounds
  function aClr(p, a) {
    var boosted = p.light ? Math.min(1, a < 0.1 ? a * 2.5 : a * 1.5) : a;
    return 'rgba(' + p.r + ',' + p.g + ',' + p.b + ',' + (boosted < 0.001 ? a : +boosted.toFixed(4)) + ')';
  }

  function drawBg(ctx, w, h, r, g, b) {
    var light = isLightTheme();
    var bg = ctx.createLinearGradient(0, 0, 0, h);
    bg.addColorStop(0, light ? '#e8e9eb' : '#0d0d0e');
    bg.addColorStop(1, light ? '#dddee0' : '#080809');
    ctx.fillStyle = bg; ctx.fillRect(0, 0, w, h);
    var glow = ctx.createRadialGradient(w*0.5, h*0.5, 0, w*0.5, h*0.5, w*0.55);
    glow.addColorStop(0, 'rgba('+r+','+g+','+b+','+(light ? 0.08 : 0.05)+')');
    glow.addColorStop(1, 'transparent');
    ctx.fillStyle = glow; ctx.fillRect(0, 0, w, h);
  }

  // ---- BOOKING: Phone mockup with medical spa content ----
  function drawBooking(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    drawBg(ctx, w, h, 184, 156, 92);
    var p = pal(184, 156, 92);

    // Subtle grid pattern
    ctx.strokeStyle = aClr(p,0.02); ctx.lineWidth = 0.5*s;
    for (var gi = 0; gi < w; gi += 20*s) { ctx.beginPath(); ctx.moveTo(gi, 0); ctx.lineTo(gi, h); ctx.stroke(); }
    for (var gj = 0; gj < h; gj += 20*s) { ctx.beginPath(); ctx.moveTo(0, gj); ctx.lineTo(w, gj); ctx.stroke(); }

    // Phone frame -larger, more prominent
    var pw = 140*s, ph = 240*s;
    var px = w*0.12, py = (h - ph)/2;

    // Phone shadow
    ctx.save(); ctx.shadowColor = (p.light ? 'rgba(26,26,28,0.16)' : 'rgba(0,0,0,0.4)'); ctx.shadowBlur = 30*s; ctx.shadowOffsetX = 6*s; ctx.shadowOffsetY = 8*s;
    roundRect(ctx, px, py, pw, ph, 18*s);
    ctx.fillStyle = p.light ? '#b8b9bc' : '#1a1a1c'; ctx.fill();
    ctx.restore();

    // Phone bezel
    roundRect(ctx, px, py, pw, ph, 18*s);
    ctx.strokeStyle = aClr(p,0.3); ctx.lineWidth = 1.5*s; ctx.stroke();

    // Dynamic island
    roundRect(ctx, px + pw*0.28, py+4*s, pw*0.44, 10*s, 5*s);
    ctx.fillStyle = p.light ? '#8a8b8e' : '#000'; ctx.fill();

    // Screen area
    var sx = px + 5*s, sy = py + 18*s, sw = pw - 10*s, sh = ph - 26*s;
    roundRect(ctx, sx, sy, sw, sh, 4*s);
    ctx.fillStyle = '#faf6f0'; ctx.fill();

    // Status bar
    ctx.fillStyle = '#efe9e0'; ctx.fillRect(sx, sy, sw, 14*s);
    ctx.fillStyle = 'rgba(80,60,30,0.5)'; ctx.font = '500 '+(6*s)+'px Inter, sans-serif';
    ctx.textAlign = 'center'; ctx.fillText('9:41', sx+sw/2, sy+10*s);
    ctx.textAlign = 'left';
    ctx.fillStyle = 'rgba(80,60,30,0.3)'; ctx.font = (5*s)+'px Inter, sans-serif';
    ctx.fillText('LTE', sx+4*s, sy+10*s);

    // App header
    ctx.fillStyle = '#f4ede4'; ctx.fillRect(sx, sy+14*s, sw, 24*s);
    ctx.fillStyle = '#8b7b60'; ctx.font = '600 '+(7*s)+'px Inter, sans-serif';
    ctx.textAlign = 'center'; ctx.fillText('BOOKING', sx+sw/2, sy+28*s);
    ctx.fillStyle = '#b0a088'; ctx.font = '300 '+(4.5*s)+'px Inter, sans-serif';
    ctx.fillText('MEDICAL SPA PLATFORM', sx+sw/2, sy+35*s);

    // Promo banner placeholder (gets replaced by real image)
    var bannerY = sy+42*s, bannerH = 44*s;
    roundRect(ctx, sx+5*s, bannerY, sw-10*s, bannerH, 4*s);
    ctx.fillStyle = '#d8cfc0'; ctx.fill();

    loadImg('thumbs/medspa-promo.jpg', function(img) {
      if (!img) return;
      ctx.save();
      roundRect(ctx, sx+5*s, bannerY, sw-10*s, bannerH, 4*s); ctx.clip();
      var ar = img.width/img.height, bw = sw-10*s, bh = bw/ar;
      if (bh < bannerH) { bh = bannerH; bw = bh*ar; }
      ctx.drawImage(img, sx+5*s+(sw-10*s-bw)/2, bannerY+(bannerH-bh)/2, bw, bh);
      ctx.restore();
    });

    // Service category cards -2x2 grid
    var services = [{t:'Facials', icon:'\u2728'}, {t:'Laser', icon:'\u26A1'}, {t:'Injectables', icon:'\u{1F489}'}, {t:'Body', icon:'\u2B50'}];
    var cardW = (sw - 16*s) / 2, cardH = 22*s;
    for (var si = 0; si < 4; si++) {
      var ccx = sx + 5*s + (si % 2) * (cardW + 6*s);
      var ccy = sy + 92*s + Math.floor(si / 2) * (cardH + 5*s);
      roundRect(ctx, ccx, ccy, cardW, cardH, 4*s);
      ctx.fillStyle = '#f0e9df'; ctx.fill();
      ctx.strokeStyle = aClr(p,0.12); ctx.lineWidth = 0.5*s; ctx.stroke();
      ctx.fillStyle = '#6b5e4a'; ctx.font = '400 '+(5.5*s)+'px Inter, sans-serif';
      ctx.textAlign = 'center'; ctx.fillText(services[si].t, ccx+cardW/2, ccy+14*s);
    }

    // Provider row with heading
    ctx.fillStyle = '#7a6c55'; ctx.font = '500 '+(5*s)+'px Inter, sans-serif';
    ctx.textAlign = 'left'; ctx.fillText('Our Providers', sx+7*s, sy+150*s);
    var provColors = ['#c4b09a','#b8a894','#a89a88','#beae9c'];
    for (var pi = 0; pi < 4; pi++) {
      var pcx = sx + 16*s + pi*24*s, pcy = sy+162*s;
      ctx.beginPath(); ctx.arc(pcx, pcy, 10*s, 0, Math.PI*2);
      ctx.fillStyle = provColors[pi]; ctx.fill();
      ctx.strokeStyle = aClr(p,0.25); ctx.lineWidth = 0.8*s; ctx.stroke();
    }
    loadImg('thumbs/medspa-provider.jpg', function(img) {
      if (!img) return;
      ctx.save();
      ctx.beginPath(); ctx.arc(sx+16*s, sy+162*s, 10*s, 0, Math.PI*2); ctx.clip();
      ctx.drawImage(img, sx+16*s-10*s, sy+162*s-10*s, 20*s, 20*s);
      ctx.restore();
    });

    // Book Now CTA
    roundRect(ctx, sx+5*s, sy+182*s, sw-10*s, 18*s, 6*s);
    var btnGrad = ctx.createLinearGradient(0, sy+182*s, 0, sy+200*s);
    btnGrad.addColorStop(0, aClr(p,0.9)); btnGrad.addColorStop(1, 'rgba(160,136,72,0.9)');
    ctx.fillStyle = btnGrad; ctx.fill();
    ctx.fillStyle = '#fff'; ctx.font = '600 '+(6*s)+'px Inter, sans-serif';
    ctx.textAlign = 'center'; ctx.fillText('Book Appointment', sx+sw/2, sy+194*s);

    // Tab bar
    ctx.fillStyle = '#f4ede4'; ctx.fillRect(sx, sy+sh-18*s, sw, 18*s);
    ctx.strokeStyle = aClr(p,0.08); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(sx, sy+sh-18*s); ctx.lineTo(sx+sw, sy+sh-18*s); ctx.stroke();
    var tabs = ['Home','Services','Book','Profile'];
    ctx.font = (4.5*s)+'px Inter, sans-serif';
    for (var ti = 0; ti < tabs.length; ti++) {
      var tx = sx + sw/(tabs.length*2) + ti*(sw/tabs.length);
      ctx.fillStyle = ti === 0 ? '#8b7b60' : '#b8a898';
      ctx.fillText(tabs[ti], tx, sy+sh-6*s);
    }

    // === Right side: elegant feature showcase ===
    var rx = w*0.5;
    ctx.textAlign = 'left';
    ctx.fillStyle = aClr(p,0.4); ctx.font = '200 '+(8*s)+'px Inter, sans-serif';
    ctx.letterSpacing = '3px';
    ctx.fillText('MEDICAL SPA PLATFORM', rx, h*0.18);
    ctx.fillStyle = tClr(p,0.75); ctx.font = '300 '+(18*s)+'px Cormorant Garamond, serif';
    ctx.fillText('Mobile', rx, h*0.29);
    ctx.fillText('Booking', rx, h*0.39);

    // Thin rule
    ctx.fillStyle = aClr(p,0.2); ctx.fillRect(rx, h*0.43, 40*s, 1);

    // Feature list with better alignment
    var features = [
      {t:'15+ Screens', d:'Complete booking flow'},
      {t:'Stripe Integration', d:'Secure payment processing'},
      {t:'Provider Calendar', d:'Real-time availability'},
      {t:'Push Alerts', d:'Appointment reminders'},
      {t:'Admin Panel', d:'Analytics & management'}
    ];
    ctx.font = (7.5*s)+'px Inter, sans-serif';
    for (var fi = 0; fi < features.length; fi++) {
      var fy = h*0.48 + fi*20*s;
      // Bullet dash
      ctx.fillStyle = aClr(p,0.4); ctx.fillText('|', rx, fy+4*s);
      ctx.fillStyle = tClr(p,0.55); ctx.font = '400 '+(7*s)+'px Inter, sans-serif';
      ctx.fillText(features[fi].t, rx+14*s, fy+4*s);
      ctx.fillStyle = tClr(p,0.2); ctx.font = '200 '+(6.5*s)+'px Inter, sans-serif';
      ctx.fillText(features[fi].d, rx+100*s, fy+4*s);
      ctx.font = (7.5*s)+'px Inter, sans-serif';
    }

    drawCorners(ctx, w, h, 14*s, aClr(p,0.15));
  }

  // ---- TRADING: Real dashboard with live-looking charts ----
  function drawTrading(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    drawBg(ctx, w, h, 58, 143, 212);
    var p = pal(58, 143, 212);

    // Header bar
    ctx.fillStyle = aClr(p,0.06); ctx.fillRect(0, 0, w, 26*s);
    ctx.strokeStyle = aClr(p,0.08); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 26*s); ctx.lineTo(w, 26*s); ctx.stroke();
    ctx.fillStyle = aClr(p,0.5); ctx.fillRect(0, 0, 3*s, 26*s);
    ctx.fillStyle = tClr(p,0.75); ctx.font = '500 '+(9*s)+'px Inter, sans-serif';
    ctx.textAlign = 'left'; ctx.fillText('Master Trade Bot', 14*s, 17*s);
    // Connection badge
    ctx.beginPath(); ctx.arc(w-12*s, 13*s, 3*s, 0, Math.PI*2);
    ctx.fillStyle = '#4ade80'; ctx.fill();
    ctx.fillStyle = aClr(p,0.4); ctx.font = (7*s)+'px monospace';
    ctx.textAlign = 'right'; ctx.fillText('LIVE', w-20*s, 17*s);
    ctx.textAlign = 'left';

    // Left panel - Engine status
    var lw = w * 0.28, lx = 6*s, ly = 32*s;
    ctx.fillStyle = aClr(p,0.025); ctx.fillRect(lx, ly, lw, h-ly-6*s);
    ctx.strokeStyle = aClr(p,0.05); ctx.lineWidth = 0.5*s; ctx.strokeRect(lx, ly, lw, h-ly-6*s);
    ctx.fillStyle = aClr(p,0.35); ctx.font = '200 '+(6*s)+'px Inter, sans-serif';
    ctx.letterSpacing = '2px'; ctx.fillText('ENGINES', lx+8*s, ly+12*s);

    var engines = [
      {name:'Solana Jupiter', status:'LIVE', color:'#4ade80'},
      {name:'Polymarket CLOB', status:'LIVE', color:'#4ade80'},
      {name:'Binance', status:'LIVE', color:'#4ade80'},
      {name:'Coinbase', status:'PAPER', color:'#fbbf24'},
      {name:'ETH DeFi', status:'IDLE', color:'#64748b'},
      {name:'Kraken', status:'PAPER', color:'#fbbf24'}
    ];
    for (var ei = 0; ei < engines.length; ei++) {
      var ey = ly + 22*s + ei * 16*s;
      ctx.beginPath(); ctx.arc(lx+12*s, ey+4*s, 2*s, 0, Math.PI*2);
      ctx.fillStyle = engines[ei].color; ctx.fill();
      ctx.fillStyle = tClr(p,0.45); ctx.font = (6.5*s)+'px Inter, sans-serif';
      ctx.textAlign = 'left'; ctx.fillText(engines[ei].name, lx+20*s, ey+7*s);
      ctx.fillStyle = engines[ei].color === '#4ade80' ? 'rgba(74,222,128,0.4)' : tClr(p,0.15);
      ctx.font = (5.5*s)+'px monospace';
      ctx.textAlign = 'right'; ctx.fillText(engines[ei].status, lx+lw-6*s, ey+7*s);
      ctx.textAlign = 'left';
    }

    // Center - Candlestick chart (deterministic seeded)
    var cx = lx + lw + 6*s, cw = w*0.44, cy = 32*s, ch = h*0.54;
    ctx.fillStyle = aClr(p,0.025); ctx.fillRect(cx, cy, cw, ch);
    ctx.strokeStyle = aClr(p,0.05); ctx.lineWidth = 0.5*s; ctx.strokeRect(cx, cy, cw, ch);

    // Chart header
    ctx.fillStyle = aClr(p,0.35); ctx.font = '200 '+(6*s)+'px Inter, sans-serif';
    ctx.fillText('BTC/USD', cx+8*s, cy+12*s);
    ctx.fillStyle = tClr(p,0.65); ctx.font = '500 '+(10*s)+'px monospace';
    ctx.fillText('$62,441.20', cx+55*s, cy+12*s);
    ctx.fillStyle = '#4ade80'; ctx.font = (7*s)+'px monospace';
    ctx.textAlign = 'right'; ctx.fillText('+1.24%', cx+cw-8*s, cy+12*s);
    ctx.textAlign = 'left';

    // Grid lines
    ctx.strokeStyle = aClr(p,0.03); ctx.lineWidth = 0.5*s;
    for (var gl = 0; gl < 5; gl++) {
      var gy = cy + 20*s + gl * (ch-30*s)/4;
      ctx.beginPath(); ctx.moveTo(cx, gy); ctx.lineTo(cx+cw, gy); ctx.stroke();
    }

    // Deterministic candlesticks
    var chartY = cy + 20*s, chartH = ch - 32*s;
    var candles = 40, seed = 62000;
    var priceData = [];
    for (var ci = 0; ci < candles; ci++) {
      seed += Math.sin(ci*1.3+0.5)*180 + Math.cos(ci*0.7+1.2)*120 + Math.sin(ci*2.1)*60;
      var open = seed, close = seed + Math.sin(ci*2.3)*280 + Math.cos(ci*1.1)*150;
      var hi = Math.max(open, close) + Math.abs(Math.sin(ci*3.1))*180;
      var lo = Math.min(open, close) - Math.abs(Math.cos(ci*2.7))*180;
      priceData.push({o:open,c:close,h:hi,l:lo});
    }
    var allPrices = priceData.reduce(function(a,c){ return a.concat([c.h,c.l]); }, []);
    var pMin = Math.min.apply(null, allPrices), pMax = Math.max.apply(null, allPrices);
    var norm = function(v) { return chartY + chartH - ((v - pMin) / (pMax - pMin)) * chartH; };

    // EMA overlay
    ctx.strokeStyle = aClr(p,0.25); ctx.lineWidth = 1*s;
    ctx.beginPath();
    var ema = priceData[0].c;
    for (var em = 0; em < candles; em++) {
      ema = ema * 0.85 + priceData[em].c * 0.15;
      var emx = cx + 8*s + em * (cw-16*s)/candles + (cw-16*s)/candles/2;
      if (em === 0) ctx.moveTo(emx, norm(ema)); else ctx.lineTo(emx, norm(ema));
    }
    ctx.stroke();

    for (var ci2 = 0; ci2 < candles; ci2++) {
      var d = priceData[ci2], isGreen = d.c > d.o;
      var cxp = cx + 8*s + ci2 * (cw-16*s)/candles;
      var barW = Math.max(1.5*s, (cw-16*s)/candles * 0.55);
      ctx.strokeStyle = isGreen ? 'rgba(74,222,128,0.35)' : 'rgba(248,113,113,0.35)';
      ctx.lineWidth = 0.8*s;
      ctx.beginPath(); ctx.moveTo(cxp+barW/2, norm(d.h)); ctx.lineTo(cxp+barW/2, norm(d.l)); ctx.stroke();
      ctx.fillStyle = isGreen ? 'rgba(74,222,128,0.55)' : 'rgba(248,113,113,0.45)';
      var bodyTop = norm(Math.max(d.o, d.c));
      ctx.fillRect(cxp, bodyTop, barW, Math.max(1, Math.abs(norm(d.o) - norm(d.c))));
    }

    // Volume bars below chart
    var vy = cy + ch + 3*s, vh = h - vy - 6*s;
    ctx.fillStyle = aClr(p,0.025); ctx.fillRect(cx, vy, cw, vh);
    ctx.strokeStyle = aClr(p,0.05); ctx.lineWidth = 0.5*s; ctx.strokeRect(cx, vy, cw, vh);
    for (var vi = 0; vi < candles; vi++) {
      var vx = cx + 8*s + vi * (cw-16*s)/candles;
      var vBarW = Math.max(1.5*s, (cw-16*s)/candles * 0.55);
      var vBarH = (0.15 + Math.abs(Math.sin(vi*1.7))*0.85) * vh * 0.75;
      var isUp = priceData[vi].c > priceData[vi].o;
      ctx.fillStyle = isUp ? 'rgba(74,222,128,0.12)' : 'rgba(248,113,113,0.1)';
      ctx.fillRect(vx, vy + vh - vBarH, vBarW, vBarH);
    }

    // Right panel - P&L + positions
    var rx = cx + cw + 6*s, rw = w - rx - 6*s;
    ctx.fillStyle = aClr(p,0.025); ctx.fillRect(rx, cy, rw, h-cy-6*s);
    ctx.strokeStyle = aClr(p,0.05); ctx.lineWidth = 0.5*s; ctx.strokeRect(rx, cy, rw, h-cy-6*s);

    // Total P&L
    ctx.fillStyle = aClr(p,0.35); ctx.font = '200 '+(6*s)+'px Inter, sans-serif';
    ctx.fillText('DAILY P&L', rx+8*s, cy+12*s);
    ctx.fillStyle = '#4ade80'; ctx.font = '500 '+(13*s)+'px monospace';
    ctx.fillText('+$2,847', rx+8*s, cy+30*s);
    ctx.fillStyle = 'rgba(74,222,128,0.4)'; ctx.font = (7*s)+'px monospace';
    ctx.fillText('+2.41%', rx+8*s, cy+42*s);

    // Divider
    ctx.fillStyle = aClr(p,0.06); ctx.fillRect(rx+6*s, cy+50*s, rw-12*s, 0.5*s);

    // Open positions
    ctx.fillStyle = aClr(p,0.3); ctx.font = '200 '+(5.5*s)+'px Inter, sans-serif';
    ctx.fillText('OPEN POSITIONS', rx+8*s, cy+62*s);
    var positions = [
      {pair:'BTC/USD', side:'LONG', pnl:'+1.8%', c:'#4ade80'},
      {pair:'SOL/USD', side:'LONG', pnl:'+3.2%', c:'#4ade80'},
      {pair:'ETH/USD', side:'FLAT', pnl:'0.0%', c:'#64748b'},
      {pair:'MATIC', side:'SHORT', pnl:'-0.4%', c:'#f87171'}
    ];
    for (var pi = 0; pi < positions.length; pi++) {
      var py = cy + 72*s + pi * 14*s;
      ctx.fillStyle = tClr(p,0.35); ctx.font = (6*s)+'px monospace';
      ctx.textAlign = 'left'; ctx.fillText(positions[pi].pair, rx+8*s, py);
      ctx.fillStyle = positions[pi].c; ctx.font = (5.5*s)+'px monospace';
      ctx.textAlign = 'right'; ctx.fillText(positions[pi].pnl, rx+rw-6*s, py);
      ctx.textAlign = 'left';
    }

    // WebSocket indicator
    ctx.fillStyle = aClr(p,0.06); ctx.fillRect(rx+6*s, h-22*s, rw-12*s, 14*s);
    ctx.fillStyle = aClr(p,0.3); ctx.font = (5*s)+'px monospace';
    ctx.textAlign = 'center'; ctx.fillText('WS CONNECTED', rx+rw/2, h-13*s);
    ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p,0.15));
  }

  // ---- PROFITDESK: Agent orchestration diagram ----
  function drawProfitDesk(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    drawBg(ctx, w, h, 180, 120, 80);
    var p = pal(180, 120, 80);

    // Title bar
    ctx.fillStyle = aClr(p,0.05); ctx.fillRect(0, 0, w, 26*s);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 26*s); ctx.lineTo(w, 26*s); ctx.stroke();
    ctx.fillStyle = aClr(p,0.5); ctx.fillRect(0, 0, 3*s, 26*s);
    ctx.fillStyle = tClr(p,0.7); ctx.font = '500 '+(9*s)+'px Inter, sans-serif';
    ctx.textAlign = 'left'; ctx.fillText('Profit Desk', 14*s, 17*s);
    ctx.fillStyle = aClr(p,0.35); ctx.font = '200 '+(7*s)+'px Inter, sans-serif';
    ctx.fillText('Multi-Agent Orchestration', 90*s, 17*s);

    // Central PM agent -larger, glowing
    var pmx = w*0.5, pmy = h*0.44;
    // Glow ring
    ctx.beginPath(); ctx.arc(pmx, pmy, 34*s, 0, Math.PI*2);
    ctx.fillStyle = aClr(p,0.04); ctx.fill();
    // Outer ring
    ctx.beginPath(); ctx.arc(pmx, pmy, 28*s, 0, Math.PI*2);
    ctx.fillStyle = aClr(p,0.1); ctx.fill();
    ctx.strokeStyle = aClr(p,0.35); ctx.lineWidth = 1.5*s; ctx.stroke();
    // Inner ring
    ctx.beginPath(); ctx.arc(pmx, pmy, 20*s, 0, Math.PI*2);
    ctx.strokeStyle = aClr(p,0.15); ctx.lineWidth = 0.5*s; ctx.stroke();
    ctx.fillStyle = tClr(p,0.75); ctx.font = '600 '+(10*s)+'px Inter, sans-serif';
    ctx.textAlign = 'center'; ctx.fillText('PM', pmx, pmy+4*s);
    ctx.fillStyle = tClr(p,0.25); ctx.font = '200 '+(5.5*s)+'px Inter, sans-serif';
    ctx.fillText('Portfolio Manager', pmx, pmy+16*s);

    // Agent nodes with animated-looking connections
    var agents = [
      {name:'Researcher', icon:'R', x:0.18, y:0.32, c:'rgba(120,180,220,'},
      {name:'Backtester', icon:'B', x:0.18, y:0.68, c:'rgba(160,200,120,'},
      {name:'Risk Mgr', icon:'$', x:0.82, y:0.32, c:'rgba(220,160,100,'},
      {name:'Exec Trader', icon:'E', x:0.82, y:0.68, c:'rgba(200,130,130,'},
      {name:'Ops Monitor', icon:'M', x:0.5, y:0.86, c:'rgba(160,140,200,'}
    ];
    for (var ai = 0; ai < agents.length; ai++) {
      var ax = w*agents[ai].x, ay = h*agents[ai].y, ac = agents[ai].c;
      // Connection line with gradient
      ctx.strokeStyle = aClr(p,0.08); ctx.lineWidth = 1*s;
      ctx.setLineDash([3*s, 5*s]);
      ctx.beginPath(); ctx.moveTo(pmx, pmy); ctx.lineTo(ax, ay); ctx.stroke();
      ctx.setLineDash([]);
      // Data flow dots along the line
      for (var dd = 0; dd < 3; dd++) {
        var t = 0.3 + dd * 0.2;
        var dx = pmx + (ax - pmx) * t, dy = pmy + (ay - pmy) * t;
        ctx.beginPath(); ctx.arc(dx, dy, 1.5*s, 0, Math.PI*2);
        ctx.fillStyle = ac + (0.15 + dd*0.1) + ')'; ctx.fill();
      }
      // Node glow
      ctx.beginPath(); ctx.arc(ax, ay, 22*s, 0, Math.PI*2);
      ctx.fillStyle = ac + '0.03)'; ctx.fill();
      // Node
      ctx.beginPath(); ctx.arc(ax, ay, 16*s, 0, Math.PI*2);
      ctx.fillStyle = ac + '0.06)'; ctx.fill();
      ctx.strokeStyle = ac + '0.25)'; ctx.lineWidth = 1*s; ctx.stroke();
      ctx.fillStyle = ac + '0.7)'; ctx.font = '600 '+(8*s)+'px Inter, sans-serif';
      ctx.textAlign = 'center'; ctx.fillText(agents[ai].icon, ax, ay+3*s);
      ctx.fillStyle = tClr(p,0.3); ctx.font = '300 '+(5.5*s)+'px Inter, sans-serif';
      ctx.fillText(agents[ai].name, ax, ay+26*s);
    }

    // Strategy pills at bottom
    ctx.textAlign = 'left';
    var strats = [
      {t:'Momentum', d:'EMA Crossover', c:'#4ade80'},
      {t:'Mean Rev', d:'BB + RSI', c:'#60a5fa'},
      {t:'Breakout', d:'Donchian', c:'#fbbf24'},
      {t:'Whale Flow', d:'UW Tracking', c:'#c084fc'}
    ];
    var stratW = (w - 24*s) / 4;
    for (var si = 0; si < strats.length; si++) {
      var sx = 8*s + si * (stratW + 4*s), sy = h - 30*s;
      roundRect(ctx, sx, sy, stratW - 4*s, 22*s, 3*s);
      ctx.fillStyle = aClr(p,0.04); ctx.fill();
      ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s; ctx.stroke();
      // Status dot
      ctx.beginPath(); ctx.arc(sx+8*s, sy+11*s, 2*s, 0, Math.PI*2);
      ctx.fillStyle = strats[si].c; ctx.fill();
      ctx.fillStyle = tClr(p,0.45); ctx.font = '400 '+(6*s)+'px Inter, sans-serif';
      ctx.fillText(strats[si].t, sx+14*s, sy+10*s);
      ctx.fillStyle = tClr(p,0.18); ctx.font = '200 '+(5*s)+'px Inter, sans-serif';
      ctx.fillText(strats[si].d, sx+14*s, sy+18*s);
    }

    drawCorners(ctx, w, h, 14*s, aClr(p,0.15));
  }

  // ---- RTS: Isometric game view ----
  function drawRTS(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    drawBg(ctx, w, h, 192, 160, 80);
    var p = pal(192, 160, 80);

    // Isometric grid -deterministic terrain
    var tileW = 30*s, tileH = 15*s;
    var ox = w*0.48, oy = h*0.15;
    for (var gy = 0; gy < 9; gy++) {
      for (var gx = 0; gx < 9; gx++) {
        var ix = ox + (gx - gy) * tileW/2;
        var iy = oy + (gx + gy) * tileH/2;
        ctx.beginPath();
        ctx.moveTo(ix, iy); ctx.lineTo(ix+tileW/2, iy+tileH/2);
        ctx.lineTo(ix, iy+tileH); ctx.lineTo(ix-tileW/2, iy+tileH/2);
        ctx.closePath();
        // Deterministic terrain
        var terrain = Math.sin(gx*2.3+gy*1.7) * 0.5 + 0.5;
        if (terrain > 0.7) {
          ctx.fillStyle = 'rgba(60,100,45,' + (0.06 + terrain*0.06) + ')'; ctx.fill(); // Forest
        } else if (terrain > 0.4) {
          ctx.fillStyle = 'rgba(80,110,50,' + (0.03 + terrain*0.03) + ')'; ctx.fill(); // Grass
        } else {
          ctx.fillStyle = 'rgba(120,105,70,0.03)'; ctx.fill(); // Sand
        }
        ctx.strokeStyle = aClr(p,0.04); ctx.lineWidth = 0.5*s; ctx.stroke();

        // Elevation blocks for buildings (deterministic)
        if ((gx === 2 && gy === 2) || (gx === 6 && gy === 5)) {
          var bx = ix, by = iy - 8*s;
          ctx.fillStyle = 'rgba(160,140,100,0.15)';
          ctx.beginPath();
          ctx.moveTo(bx, by); ctx.lineTo(bx+tileW/2, by+tileH/2);
          ctx.lineTo(bx+tileW/2, by+tileH/2+8*s); ctx.lineTo(bx, by+tileH+8*s);
          ctx.lineTo(bx-tileW/2, by+tileH/2+8*s); ctx.lineTo(bx-tileW/2, by+tileH/2);
          ctx.closePath(); ctx.fill();
          ctx.strokeStyle = aClr(p,0.12); ctx.stroke();
        }
      }
    }

    // Units -blue team and red team (deterministic positions)
    var blueUnits = [[2,3],[3,3],[2,4],[3,4],[1,3]];
    var redUnits = [[6,5],[5,6],[6,6],[7,5],[7,6]];
    function drawUnit(gx, gy, color, outline) {
      var ux = ox + (gx - gy) * tileW/2;
      var uy = oy + (gx + gy) * tileH/2 + tileH/2;
      // Shadow
      ctx.beginPath(); ctx.ellipse(ux, uy+2*s, 3*s, 1.5*s, 0, 0, Math.PI*2);
      ctx.fillStyle = (p.light ? 'rgba(26,26,28,0.08)' : 'rgba(0,0,0,0.2)'); ctx.fill();
      // Unit body
      ctx.beginPath(); ctx.arc(ux, uy-2*s, 3*s, 0, Math.PI*2);
      ctx.fillStyle = color; ctx.fill();
      ctx.strokeStyle = outline; ctx.lineWidth = 0.5*s; ctx.stroke();
    }
    for (var bi2 = 0; bi2 < blueUnits.length; bi2++) drawUnit(blueUnits[bi2][0], blueUnits[bi2][1], 'rgba(80,160,255,0.7)', 'rgba(120,190,255,0.5)');
    for (var ri = 0; ri < redUnits.length; ri++) drawUnit(redUnits[ri][0], redUnits[ri][1], 'rgba(255,90,90,0.7)', 'rgba(255,140,140,0.5)');

    // Selection box around blue units
    var selX = ox + (1-4)*tileW/2, selY = oy + (1+3)*tileH/2;
    ctx.strokeStyle = 'rgba(80,160,255,0.3)'; ctx.lineWidth = 1*s;
    ctx.setLineDash([3*s, 2*s]);
    ctx.strokeRect(selX-10*s, selY-6*s, 80*s, 50*s);
    ctx.setLineDash([]);

    // HUD -top bar
    ctx.fillStyle = (p.light ? 'rgba(26,26,28,0.26)' : 'rgba(0,0,0,0.65)'); ctx.fillRect(0, 0, w, 22*s);
    ctx.fillStyle = aClr(p,0.8); ctx.font = '600 '+(8*s)+'px Inter, sans-serif';
    ctx.textAlign = 'left'; ctx.fillText('Bloodlines', 10*s, 15*s);
    // Resource icons
    ctx.textAlign = 'right'; ctx.font = (6.5*s)+'px monospace';
    ctx.fillStyle = '#ffd700'; ctx.fillText('1,250', w-10*s, 10*s);
    ctx.fillStyle = tClr(p,0.3); ctx.fillText('Gold', w-50*s, 10*s);
    ctx.fillStyle = '#87ceeb'; ctx.fillText('48', w-10*s, 20*s);
    ctx.fillStyle = tClr(p,0.3); ctx.fillText('Units', w-30*s, 20*s);
    ctx.fillStyle = tClr(p,0.25); ctx.fillText('Pop: 32/50', w-60*s, 20*s);

    // Minimap corner
    var mmx = w-65*s, mmy = h-55*s, mmw = 56*s, mmh = 46*s;
    ctx.fillStyle = (p.light ? 'rgba(26,26,28,0.22)' : 'rgba(0,0,0,0.55)');
    ctx.fillRect(mmx, mmy, mmw, mmh);
    ctx.strokeStyle = aClr(p,0.25); ctx.lineWidth = 1*s; ctx.strokeRect(mmx, mmy, mmw, mmh);
    // Terrain blotches
    ctx.fillStyle = 'rgba(60,100,45,0.2)'; ctx.fillRect(mmx+8*s, mmy+6*s, 18*s, 14*s);
    ctx.fillStyle = 'rgba(60,100,45,0.15)'; ctx.fillRect(mmx+28*s, mmy+20*s, 16*s, 12*s);
    // Unit clusters
    for (var md = 0; md < 5; md++) {
      ctx.beginPath(); ctx.arc(mmx+12*s+md*3*s, mmy+18*s, 1.5*s, 0, Math.PI*2);
      ctx.fillStyle = 'rgba(80,160,255,0.6)'; ctx.fill();
    }
    for (var md2 = 0; md2 < 5; md2++) {
      ctx.beginPath(); ctx.arc(mmx+34*s+md2*3*s, mmy+30*s, 1.5*s, 0, Math.PI*2);
      ctx.fillStyle = 'rgba(255,90,90,0.6)'; ctx.fill();
    }
    // Viewport indicator
    ctx.strokeStyle = 'rgba(255,255,255,0.2)'; ctx.lineWidth = 0.5*s;
    ctx.strokeRect(mmx+8*s, mmy+10*s, 20*s, 16*s);

    // Bottom action bar
    ctx.fillStyle = (p.light ? 'rgba(26,26,28,0.24)' : 'rgba(0,0,0,0.6)'); ctx.fillRect(0, h-26*s, w*0.65, 26*s);
    var actions = ['Move', 'Attack', 'Build', 'Gather', 'Patrol', 'Stop'];
    ctx.font = (6*s)+'px Inter, sans-serif'; ctx.textAlign = 'center';
    for (var ab = 0; ab < actions.length; ab++) {
      var abx = 28*s + ab * 38*s;
      roundRect(ctx, abx-15*s, h-21*s, 30*s, 14*s, 2*s);
      ctx.fillStyle = ab === 0 ? aClr(p,0.15) : aClr(p,0.06); ctx.fill();
      ctx.strokeStyle = ab === 0 ? aClr(p,0.3) : aClr(p,0.12); ctx.lineWidth = 0.5*s; ctx.stroke();
      ctx.fillStyle = ab === 0 ? tClr(p,0.6) : tClr(p,0.3); ctx.fillText(actions[ab], abx, h-11*s);
    }
    ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p,0.15));
  }

  // ---- PLATFORMER: 3D game scene ----
  function drawPlatformer(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(106, 196, 74);

    // Rich sky gradient
    var sky = ctx.createLinearGradient(0, 0, 0, h);
    if (p.light) { sky.addColorStop(0, '#b8d4f0'); sky.addColorStop(0.3, '#a0c8e8'); sky.addColorStop(0.6, '#90bbe0'); sky.addColorStop(1, '#a8d8b8'); }
    else { sky.addColorStop(0, '#0c1628'); sky.addColorStop(0.3, '#1a2d4a'); sky.addColorStop(0.6, '#243a58'); sky.addColorStop(1, '#162a1e'); }
    ctx.fillStyle = sky; ctx.fillRect(0, 0, w, h);

    // Stars (deterministic)
    for (var si = 0; si < 40; si++) {
      var sx2 = (Math.sin(si*7.3+2.1)*0.5+0.5)*w;
      var sy2 = (Math.sin(si*3.7+1.4)*0.5+0.5)*h*0.4;
      var sr = (0.4 + Math.sin(si*4.2)*0.3)*s;
      ctx.beginPath(); ctx.arc(sx2, sy2, sr, 0, Math.PI*2);
      ctx.fillStyle = 'rgba(255,255,255,' + (0.1+Math.sin(si*2.9)*0.15) + ')'; ctx.fill();
    }

    // Background mountains
    ctx.beginPath(); ctx.moveTo(0, h*0.55);
    for (var mi = 0; mi <= 10; mi++) {
      var mx = mi/10*w, my = h*0.55 - Math.sin(mi*1.2+0.5)*h*0.12 - Math.cos(mi*0.8)*h*0.05;
      ctx.lineTo(mx, my);
    }
    ctx.lineTo(w, h); ctx.lineTo(0, h); ctx.closePath();
    ctx.fillStyle = p.light ? 'rgba(60,100,70,0.15)' : 'rgba(20,40,30,0.4)'; ctx.fill();

    // Ground platforms with 3D depth
    var platforms = [
      {x:0.02, y:0.78, w:0.28, main:true}, {x:0.34, y:0.62, w:0.22}, {x:0.62, y:0.5, w:0.16},
      {x:0.8, y:0.66, w:0.18}, {x:0.18, y:0.42, w:0.14}, {x:0.48, y:0.36, w:0.12}
    ];
    for (var pi = 0; pi < platforms.length; pi++) {
      var plat = platforms[pi];
      var ppx = plat.x*w, ppy = plat.y*h, ppw = plat.w*w, pph = 10*s;
      var depth = 6*s;
      // Front face (depth)
      ctx.fillStyle = '#1e4a16'; ctx.fillRect(ppx, ppy+pph, ppw, depth);
      // Top surface
      var pg = ctx.createLinearGradient(0, ppy, 0, ppy+pph);
      pg.addColorStop(0, '#4a9a38'); pg.addColorStop(1, '#2a7020');
      ctx.fillStyle = pg; ctx.fillRect(ppx, ppy, ppw, pph);
      // Grass highlights
      ctx.fillStyle = '#6ad44a'; ctx.fillRect(ppx, ppy, ppw, 2*s);
      // Grass detail tufts
      for (var gt = 0; gt < ppw/(6*s); gt++) {
        var gx = ppx + gt*6*s + 3*s;
        ctx.fillStyle = 'rgba(110,220,70,0.3)';
        ctx.fillRect(gx, ppy-1*s, 1*s, 2*s);
      }
    }

    // Player character -more detailed
    var playerX = w*0.37, playerY = h*0.54;
    // Shadow
    ctx.beginPath(); ctx.ellipse(playerX, playerY+14*s, 6*s, 2*s, 0, 0, Math.PI*2);
    ctx.fillStyle = (p.light ? 'rgba(26,26,28,0.12)' : 'rgba(0,0,0,0.3)'); ctx.fill();
    // Legs
    ctx.fillStyle = '#2255aa'; ctx.fillRect(playerX-4*s, playerY+8*s, 3*s, 6*s);
    ctx.fillRect(playerX+1*s, playerY+8*s, 3*s, 6*s);
    // Body
    ctx.fillStyle = '#4488ff'; ctx.fillRect(playerX-5*s, playerY, 10*s, 10*s);
    // Cape/trail
    ctx.fillStyle = 'rgba(255,80,80,0.5)';
    ctx.beginPath(); ctx.moveTo(playerX+5*s, playerY+2*s); ctx.lineTo(playerX+12*s, playerY+8*s);
    ctx.lineTo(playerX+5*s, playerY+10*s); ctx.closePath(); ctx.fill();
    // Head
    ctx.beginPath(); ctx.arc(playerX, playerY-4*s, 6*s, 0, Math.PI*2);
    ctx.fillStyle = '#ffcc88'; ctx.fill();
    ctx.strokeStyle = 'rgba(200,160,100,0.3)'; ctx.lineWidth = 0.5*s; ctx.stroke();
    // Eyes
    ctx.fillStyle = '#333'; ctx.fillRect(playerX-3*s, playerY-5*s, 2*s, 2.5*s);
    ctx.fillRect(playerX+1*s, playerY-5*s, 2*s, 2.5*s);
    // Eye shine
    ctx.fillStyle = '#fff'; ctx.fillRect(playerX-2.5*s, playerY-5*s, 1*s, 1*s);
    ctx.fillRect(playerX+1.5*s, playerY-5*s, 1*s, 1*s);

    // Collectible coins with glow
    var coins = [[0.55, 0.43], [0.60, 0.43], [0.65, 0.43], [0.20, 0.34], [0.26, 0.34]];
    for (var ci = 0; ci < coins.length; ci++) {
      var ccx = coins[ci][0]*w, ccy = coins[ci][1]*h;
      // Glow
      ctx.beginPath(); ctx.arc(ccx, ccy, 6*s, 0, Math.PI*2);
      ctx.fillStyle = 'rgba(255,215,0,0.1)'; ctx.fill();
      // Coin
      ctx.beginPath(); ctx.arc(ccx, ccy, 4*s, 0, Math.PI*2);
      ctx.fillStyle = '#ffd700'; ctx.fill();
      ctx.strokeStyle = '#daa520'; ctx.lineWidth = 0.8*s; ctx.stroke();
      // $ symbol
      ctx.fillStyle = '#b8860b'; ctx.font = 'bold '+(4*s)+'px Inter, sans-serif';
      ctx.textAlign = 'center'; ctx.fillText('$', ccx, ccy+1.5*s);
    }

    // HUD overlay -top bar
    var hudGrad = ctx.createLinearGradient(0, 0, 0, 28*s);
    hudGrad.addColorStop(0, (p.light ? 'rgba(26,26,28,0.28)' : 'rgba(0,0,0,0.7)')); hudGrad.addColorStop(1, (p.light ? 'rgba(26,26,28,0.12)' : 'rgba(0,0,0,0.3)'));
    ctx.fillStyle = hudGrad; ctx.fillRect(0, 0, w, 28*s);

    ctx.fillStyle = '#6ad44a'; ctx.font = 'bold '+(9*s)+'px Inter, sans-serif';
    ctx.textAlign = 'left'; ctx.fillText('Jump Quest!', 10*s, 17*s);

    // XP bar with rounded edges
    var xpX = w*0.32, xpW = w*0.32;
    roundRect(ctx, xpX, 7*s, xpW, 12*s, 6*s);
    ctx.fillStyle = 'rgba(255,255,255,0.08)'; ctx.fill();
    roundRect(ctx, xpX, 7*s, xpW*0.65, 12*s, 6*s);
    ctx.fillStyle = 'rgba(106,212,74,0.5)'; ctx.fill();
    ctx.fillStyle = 'rgba(255,255,255,0.6)'; ctx.font = '500 '+(5.5*s)+'px Inter, sans-serif';
    ctx.textAlign = 'center'; ctx.fillText('LVL 7 | 1,240 / 1,900 XP', xpX+xpW/2, 15.5*s);

    // Coins and hearts
    ctx.textAlign = 'right';
    ctx.fillStyle = '#ffd700'; ctx.font = '600 '+(8*s)+'px Inter, sans-serif';
    ctx.fillText('350', w-12*s, 13*s);
    ctx.fillStyle = '#ff4466'; ctx.font = (8*s)+'px Inter, sans-serif';
    ctx.fillText('\u2764 \u2764 \u2764', w-12*s, 24*s);

    // Controller hints -bottom right
    roundRect(ctx, w-85*s, h-22*s, 78*s, 16*s, 4*s);
    ctx.fillStyle = (p.light ? 'rgba(26,26,28,0.18)' : 'rgba(0,0,0,0.45)'); ctx.fill();
    ctx.fillStyle = 'rgba(255,255,255,0.35)'; ctx.font = (5.5*s)+'px Inter, sans-serif';
    ctx.textAlign = 'center'; ctx.fillText('A Jump   X Boost   Y Shield', w-46*s, h-11.5*s);
    ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p,0.15));
  }

  // ---- MESSENGER: Chat UI ----
  function drawMessenger(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    drawBg(ctx, w, h, 138, 106, 212);
    var p = pal(138, 106, 212);

    // App header bar (window chrome)
    ctx.fillStyle = aClr(p,0.05); ctx.fillRect(0, 0, w, 22*s);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 22*s); ctx.lineTo(w, 22*s); ctx.stroke();
    ctx.fillStyle = aClr(p,0.5); ctx.fillRect(0, 0, 3*s, 22*s);
    ctx.fillStyle = tClr(p,0.7); ctx.font = '500 '+(8*s)+'px Inter, sans-serif';
    ctx.fillText('Sovereign Signal', 14*s, 14*s);
    // Lock badge (encrypted)
    ctx.fillStyle = aClr(p,0.45); ctx.font = (6*s)+'px Inter, sans-serif';
    ctx.textAlign = 'right'; ctx.fillText('\uD83D\uDD12 E2EE', w-12*s, 14*s);
    ctx.textAlign = 'left';

    // ----- Conversation list sidebar (left) -----
    var sideX = 0, sideY = 22*s, sideW = w*0.36, sideH = h - sideY;
    ctx.fillStyle = aClr(p,0.025); ctx.fillRect(sideX, sideY, sideW, sideH);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(sideX+sideW, sideY); ctx.lineTo(sideX+sideW, h); ctx.stroke();

    // Search box
    var searchY = sideY + 6*s;
    roundRect(ctx, sideX+6*s, searchY, sideW-12*s, 14*s, 4*s);
    ctx.fillStyle = aClr(p,0.04); ctx.fill();
    ctx.strokeStyle = aClr(p,0.07); ctx.lineWidth = 0.5*s; ctx.stroke();
    ctx.fillStyle = tClr(p,0.18); ctx.font = '300 '+(5*s)+'px Inter, sans-serif';
    ctx.fillText('Search...', sideX+12*s, searchY+9.5*s);

    // 6 conversations
    var convs = [
      {name:'Alex Chen',     last:'Ship it',                  time:'2:44',  unread:0, active:true,  color:'#a78bfa'},
      {name:'Maria Vasquez', last:'X3DH handshake works',     time:'2:31',  unread:2, active:false, color:'#f472b6'},
      {name:'Dev Channel',   last:'Sarah: PR ready for review', time:'1:58', unread:5, active:false, color:'#60a5fa'},
      {name:'Trading Desk',  last:'Liam: DRY mode confirmed', time:'12:14', unread:0, active:false, color:'#34d399'},
      {name:'Mom',           last:'Call me when you can',     time:'11:02', unread:1, active:false, color:'#fbbf24'},
      {name:'Sovereign Ops', last:'Auto-sync complete',       time:'Yest',  unread:0, active:false, color:'#94a3b8'}
    ];
    var convStartY = searchY + 18*s;
    var convH = (sideH - (convStartY - sideY) - 6*s) / convs.length;
    convH = Math.min(convH, 26*s);
    for (var ci = 0; ci < convs.length; ci++) {
      var c = convs[ci];
      var cy = convStartY + ci*convH;
      // Active highlight
      if (c.active) {
        ctx.fillStyle = aClr(p,0.08); ctx.fillRect(sideX, cy, sideW, convH);
        ctx.fillStyle = aClr(p,0.55); ctx.fillRect(sideX, cy, 2*s, convH);
      }
      // Avatar
      ctx.beginPath(); ctx.arc(sideX+12*s, cy+convH/2, 7*s, 0, Math.PI*2);
      ctx.fillStyle = c.color; ctx.globalAlpha = 0.25; ctx.fill();
      ctx.strokeStyle = c.color; ctx.globalAlpha = 0.5; ctx.lineWidth = 0.6*s; ctx.stroke();
      ctx.globalAlpha = 1;
      ctx.fillStyle = c.color; ctx.globalAlpha = 0.7;
      ctx.font = '600 '+(5.5*s)+'px Inter, sans-serif';
      ctx.textAlign = 'center'; ctx.fillText(c.name.charAt(0), sideX+12*s, cy+convH/2+2*s);
      ctx.textAlign = 'left'; ctx.globalAlpha = 1;
      // Name
      ctx.fillStyle = tClr(p, c.active ? 0.7 : 0.5);
      ctx.font = (c.active ? '500 ' : '400 ')+(5.5*s)+'px Inter, sans-serif';
      ctx.fillText(c.name, sideX+22*s, cy+convH/2-1*s);
      // Last message preview
      ctx.fillStyle = tClr(p,0.22); ctx.font = '300 '+(4.5*s)+'px Inter, sans-serif';
      var preview = c.last.length > 22 ? c.last.substring(0,21)+'\u2026' : c.last;
      ctx.fillText(preview, sideX+22*s, cy+convH/2+8*s);
      // Time
      ctx.fillStyle = tClr(p,0.2); ctx.font = (4*s)+'px monospace';
      ctx.textAlign = 'right'; ctx.fillText(c.time, sideX+sideW-6*s, cy+convH/2-1*s);
      ctx.textAlign = 'left';
      // Unread badge
      if (c.unread > 0) {
        ctx.beginPath(); ctx.arc(sideX+sideW-9*s, cy+convH/2+6*s, 4*s, 0, Math.PI*2);
        ctx.fillStyle = aClr(p,0.7); ctx.fill();
        ctx.fillStyle = (p.light ? 'rgba(255,255,255,0.95)' : 'rgba(13,13,14,0.95)');
        ctx.font = '600 '+(4*s)+'px Inter, sans-serif';
        ctx.textAlign = 'center'; ctx.fillText(String(c.unread), sideX+sideW-9*s, cy+convH/2+8*s);
        ctx.textAlign = 'left';
      }
    }

    // ----- Thread area (right) -----
    var threadX = sideW, threadY = 22*s, threadW = w - threadX, threadH = h - threadY;

    // Thread header (selected contact)
    ctx.fillStyle = aClr(p,0.03); ctx.fillRect(threadX, threadY, threadW, 22*s);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(threadX, threadY+22*s); ctx.lineTo(w, threadY+22*s); ctx.stroke();
    // Avatar in header
    ctx.beginPath(); ctx.arc(threadX+14*s, threadY+11*s, 7*s, 0, Math.PI*2);
    ctx.fillStyle = '#a78bfa'; ctx.globalAlpha = 0.25; ctx.fill();
    ctx.globalAlpha = 1;
    ctx.fillStyle = 'rgba(167,139,250,0.7)'; ctx.font = '600 '+(6*s)+'px Inter, sans-serif';
    ctx.textAlign = 'center'; ctx.fillText('A', threadX+14*s, threadY+13*s);
    ctx.textAlign = 'left';
    // Name + presence
    ctx.fillStyle = tClr(p,0.65); ctx.font = '500 '+(7*s)+'px Inter, sans-serif';
    ctx.fillText('Alex Chen', threadX+25*s, threadY+10*s);
    ctx.beginPath(); ctx.arc(threadX+27*s, threadY+17*s, 1.5*s, 0, Math.PI*2);
    ctx.fillStyle = '#4ade80'; ctx.fill();
    ctx.fillStyle = aClr(p,0.4); ctx.font = '200 '+(5*s)+'px Inter, sans-serif';
    ctx.fillText('Online', threadX+31*s, threadY+18*s);

    // Chat messages
    var chatStartY = threadY + 28*s;
    var msgs = [
      {in:true,  text:'Build looks solid',         time:'2:41'},
      {in:false, text:'Pushed the auth flow',      time:'2:42'},
      {in:true,  text:'X3DH handshake works',      time:'2:42'},
      {in:false, text:'Double Ratchet rekeying',   time:'2:43'},
      {in:true,  text:'Ship it',                    time:'2:44'}
    ];
    var my = chatStartY;
    for (var mi = 0; mi < msgs.length; mi++) {
      var msg = msgs[mi];
      ctx.font = (5.5*s)+'px Inter, sans-serif';
      var textW = ctx.measureText(msg.text).width;
      var mw = Math.min(threadW*0.7, textW + 16*s);
      mw = Math.max(mw, 32*s);
      var mh = 14*s;
      var mx = msg.in ? threadX+8*s : threadX+threadW-mw-8*s;

      roundRect(ctx, mx, my, mw, mh, 5*s);
      ctx.fillStyle = msg.in ? aClr(p,0.06) : aClr(p,0.16);
      ctx.fill();
      ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s; ctx.stroke();

      ctx.fillStyle = msg.in ? tClr(p,0.5) : tClr(p,0.6);
      ctx.font = '300 '+(5.5*s)+'px Inter, sans-serif';
      ctx.textAlign = 'left'; ctx.fillText(msg.text, mx+6*s, my+9*s);

      ctx.fillStyle = tClr(p,0.18); ctx.font = '200 '+(3.8*s)+'px Inter, sans-serif';
      ctx.textAlign = 'right'; ctx.fillText(msg.time, mx+mw-4*s, my+12*s);
      ctx.textAlign = 'left';

      my += mh + 4*s;
      if (my > h - 32*s) break;
    }

    // Input bar (thread area only)
    var inH = 16*s;
    roundRect(ctx, threadX+8*s, h-inH-8*s, threadW-16*s, inH, 6*s);
    ctx.fillStyle = aClr(p,0.04); ctx.fill();
    ctx.strokeStyle = aClr(p,0.08); ctx.lineWidth = 0.5*s; ctx.stroke();
    ctx.fillStyle = tClr(p,0.16); ctx.font = '200 '+(5.5*s)+'px Inter, sans-serif';
    ctx.fillText('Message...', threadX+16*s, h-inH);
    // Send button
    ctx.beginPath(); ctx.arc(w-16*s, h-inH/2-8*s, 5.5*s, 0, Math.PI*2);
    ctx.fillStyle = aClr(p,0.25); ctx.fill();
    ctx.fillStyle = aClr(p,0.6); ctx.font = (5*s)+'px Inter, sans-serif';
    ctx.textAlign = 'center'; ctx.fillText('\u2191', w-16*s, h-inH/2-6*s);
    ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p,0.15));
  }

  // ---- POLYMARKET: Terminal view ----
  function drawPolymarket(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(0, 200, 80);
    ctx.fillStyle = p.light ? '#e8e9eb' : '#020204'; ctx.fillRect(0, 0, w, h);

    // CRT scanline effect
    for (var sl = 0; sl < h; sl += 3*s) {
      ctx.fillStyle = p.light ? 'rgba(0,140,60,0.012)' : 'rgba(0,255,100,0.006)'; ctx.fillRect(0, sl, w, 1.5*s);
    }
    // CRT vignette
    var vig = ctx.createRadialGradient(w/2, h/2, w*0.25, w/2, h/2, w*0.7);
    vig.addColorStop(0, 'transparent'); vig.addColorStop(1, (p.light ? 'rgba(26,26,28,0.12)' : 'rgba(0,0,0,0.3)'));
    ctx.fillStyle = vig; ctx.fillRect(0, 0, w, h);

    // Terminal content
    ctx.font = (7*s)+'px monospace'; ctx.textAlign = 'left';
    var lh = 11*s;
    var lines = [
      {c:(p.light ? 'rgba(0,140,90,0.8)' : 'rgba(0,200,128,0.8)'),  t:'[PolyBTC] 15-Min Market Analysis'},
      {c:(p.light ? 'rgba(26,26,28,0.4)' : 'rgba(100,100,100,0.4)'),t:'────────────────────────────────────────'},
      {c:(p.light ? 'rgba(0,120,70,0.6)' : 'rgba(0,170,100,0.6)'), t:'  Chainlink BTC/USD   $62,441.20'},
      {c:(p.light ? 'rgba(0,120,70,0.6)' : 'rgba(0,170,100,0.6)'), t:'  Binance Spot        $62,438.50  (-$2.70)'},
      {c:(p.light ? 'rgba(26,26,28,0.4)' : 'rgba(80,80,80,0.4)'),  t:''},
      {c:(p.light ? 'rgba(0,140,90,0.7)' : 'rgba(0,200,128,0.7)'), t:'  Polymarket Contracts:'},
      {c:'rgba(74,222,128,0.7)',t:'    BTC Up   \u2192  0.54  \u25B2 +0.02  [65% conf]'},
      {c:'rgba(248,113,113,0.6)',t:'    BTC Down \u2192  0.46  \u25BC -0.02  [35% conf]'},
      {c:(p.light ? 'rgba(26,26,28,0.4)' : 'rgba(80,80,80,0.4)'),  t:''},
      {c:(p.light ? 'rgba(0,140,90,0.6)' : 'rgba(0,200,128,0.6)'), t:'  Indicators:'},
      {c:(p.light ? 'rgba(60,60,60,0.5)' : 'rgba(140,140,140,0.5)'),t:'    RSI(14)   58.2   Neutral'},
      {c:'rgba(74,222,128,0.6)',t:'    MACD      Bullish crossover \u2713'},
      {c:(p.light ? 'rgba(60,60,60,0.5)' : 'rgba(140,140,140,0.5)'),t:'    VWAP      $62,300 (above)'},
      {c:'rgba(74,222,128,0.5)',t:'    H.Ashi    Green forming'},
      {c:(p.light ? 'rgba(26,26,28,0.4)' : 'rgba(80,80,80,0.4)'),  t:''},
      {c:'rgba(251,191,36,0.7)',t:'  \u25B6 Signal: LEAN UP  |  Confidence: 65%'},
      {c:(p.light ? 'rgba(26,26,28,0.4)' : 'rgba(80,80,80,0.4)'),  t:''},
      {c:(p.light ? 'rgba(0,140,90,0.5)' : 'rgba(0,200,128,0.5)'), t:'  [CopyBot] Watching whale_0x7a...'},
      {c:(p.light ? 'rgba(0,140,90,0.4)' : 'rgba(0,200,128,0.4)'), t:'  [CopyBot] Last mirror: BUY 0.54 @ 12:45'},
    ];
    for (var li = 0; li < lines.length; li++) {
      ctx.fillStyle = lines[li].c; ctx.fillText(lines[li].t, 8*s, 12*s + li*lh);
    }

    // Blinking cursor
    ctx.fillStyle = aClr(p,0.5); ctx.fillRect(8*s, 12*s + lines.length*lh, 5*s, 8*s);

    // Mini sparkline chart in top-right
    var spx = w-90*s, spy = 8*s, spw = 82*s, sph = 40*s;
    ctx.fillStyle = aClr(p,0.03); ctx.fillRect(spx, spy, spw, sph);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s; ctx.strokeRect(spx, spy, spw, sph);
    ctx.fillStyle = aClr(p,0.25); ctx.font = (5*s)+'px monospace';
    ctx.fillText('BTC 15m', spx+2*s, spy+8*s);
    // Sparkline
    ctx.strokeStyle = aClr(p,0.35); ctx.lineWidth = 1*s;
    ctx.beginPath();
    for (var sp = 0; sp < 30; sp++) {
      var spXp = spx + 4*s + sp * (spw-8*s)/30;
      var spYp = spy + sph*0.3 + Math.sin(sp*0.4+1)*sph*0.15 + Math.cos(sp*0.7)*sph*0.1;
      if (sp === 0) ctx.moveTo(spXp, spYp); else ctx.lineTo(spXp, spYp);
    }
    ctx.stroke();
    // Fill under
    ctx.lineTo(spx+spw-4*s, spy+sph); ctx.lineTo(spx+4*s, spy+sph); ctx.closePath();
    ctx.fillStyle = aClr(p,0.04); ctx.fill();

    drawCorners(ctx, w, h, 14*s, aClr(p,0.15));
  }

  // ---- GALLEON: Loads real ship image ----
  function drawGalleon(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(120, 150, 180);

    // Deep storm sky
    var sky = ctx.createLinearGradient(0, 0, 0, h);
    if (p.light) { sky.addColorStop(0, '#dde2e8'); sky.addColorStop(0.3, '#d8dee6'); sky.addColorStop(0.7, '#d5dce5'); sky.addColorStop(1, '#dde2e8'); }
    else { sky.addColorStop(0, '#060a10'); sky.addColorStop(0.3, '#0e141c'); sky.addColorStop(0.7, '#101820'); sky.addColorStop(1, '#060a0e'); }
    ctx.fillStyle = sky; ctx.fillRect(0, 0, w, h);

    // Storm clouds
    for (var ci = 0; ci < 5; ci++) {
      var cx2 = (Math.sin(ci*2.3)*0.3+0.5)*w, cy2 = ci*h*0.08;
      var cg = ctx.createRadialGradient(cx2, cy2, 0, cx2, cy2, w*0.2);
      cg.addColorStop(0, p.light ? 'rgba(120,150,180,0.06)' : 'rgba(30,40,55,0.15)'); cg.addColorStop(1, 'transparent');
      ctx.fillStyle = cg; ctx.fillRect(0, 0, w, h*0.5);
    }

    // Lightning bolt with glow
    ctx.save();
    // Broad glow
    ctx.strokeStyle = 'rgba(180,200,240,0.04)'; ctx.lineWidth = 16*s;
    ctx.beginPath(); ctx.moveTo(w*0.72, 0); ctx.lineTo(w*0.66, h*0.22);
    ctx.lineTo(w*0.73, h*0.18); ctx.lineTo(w*0.58, h*0.48); ctx.stroke();
    // Tight glow
    ctx.strokeStyle = 'rgba(200,215,240,0.08)'; ctx.lineWidth = 6*s; ctx.stroke();
    // Core
    ctx.strokeStyle = 'rgba(220,230,255,0.2)'; ctx.lineWidth = 1.5*s; ctx.stroke();
    ctx.restore();

    // Matrix rain (deterministic)
    for (var mi = 0; mi < 20; mi++) {
      var mx = (Math.sin(mi*5.7+0.3)*0.5+0.5)*w;
      var my = (Math.sin(mi*3.2+1.1)*0.5+0.5)*h*0.25;
      var ml = (0.2+Math.sin(mi*2.1)*0.2)*h;
      var mg = ctx.createLinearGradient(mx, my, mx, my+ml);
      mg.addColorStop(0, (p.light ? 'rgba(40,120,100,0)' : 'rgba(64,200,160,0)'));
      mg.addColorStop(0.5, (p.light ? 'rgba(40,120,100,'+(0.03+Math.sin(mi*1.7)*0.02)+')' : 'rgba(64,200,160,'+(0.03+Math.sin(mi*1.7)*0.02)+')'));
      mg.addColorStop(1, (p.light ? 'rgba(40,120,100,0)' : 'rgba(64,200,160,0)'));
      ctx.strokeStyle = mg; ctx.lineWidth = 0.8*s;
      ctx.beginPath(); ctx.moveTo(mx, my); ctx.lineTo(mx, my+ml); ctx.stroke();
    }

    // Ocean water
    var ocean = ctx.createLinearGradient(0, h*0.75, 0, h);
    ocean.addColorStop(0, p.light ? 'rgba(120,150,180,0.08)' : 'rgba(10,30,50,0.3)'); ocean.addColorStop(1, p.light ? 'rgba(120,150,180,0.15)' : 'rgba(5,15,25,0.8)');
    ctx.fillStyle = ocean; ctx.fillRect(0, h*0.75, w, h*0.25);
    // Wave lines
    ctx.strokeStyle = p.light ? 'rgba(80,120,160,0.1)' : 'rgba(80,120,160,0.06)'; ctx.lineWidth = 0.5*s;
    for (var wv = 0; wv < 4; wv++) {
      ctx.beginPath();
      for (var wx = 0; wx < w; wx += 2*s) {
        var wy = h*0.78 + wv*10*s + Math.sin(wx/(20*s)+wv*1.5)*3*s;
        if (wx === 0) ctx.moveTo(wx, wy); else ctx.lineTo(wx, wy);
      }
      ctx.stroke();
    }

    // Load real ship image
    loadImg('thumbs/galleon-ship.png', function(img) {
      if (!img) return;
      var iw = w*0.55, ih = iw * (img.height/img.width);
      ctx.globalAlpha = 0.45;
      ctx.drawImage(img, (w-iw)/2, h-ih-5*s, iw, ih);
      ctx.globalAlpha = 1.0;
      // Fade bottom into ocean
      var fade = ctx.createLinearGradient(0, h-50*s, 0, h);
      fade.addColorStop(0, p.light ? 'rgba(221,226,232,0)' : 'rgba(6,10,14,0)'); fade.addColorStop(1, p.light ? 'rgba(221,226,232,1)' : 'rgba(6,10,14,1)');
      ctx.fillStyle = fade; ctx.fillRect(0, h-50*s, w, 50*s);
      // Re-draw corners after async
      drawCorners(ctx, w, h, 14*s, aClr(p,0.15));
    });

    // HUD overlay
    ctx.fillStyle = (p.light ? 'rgba(40,120,100,0.06)' : 'rgba(64,200,160,0.06)'); ctx.fillRect(0, 0, w, 20*s);
    ctx.strokeStyle = (p.light ? 'rgba(40,120,100,0.06)' : 'rgba(64,200,160,0.06)'); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 20*s); ctx.lineTo(w, 20*s); ctx.stroke();
    ctx.fillStyle = (p.light ? 'rgba(40,120,100,0.35)' : 'rgba(64,200,160,0.35)'); ctx.font = '200 '+(6*s)+'px monospace';
    ctx.textAlign = 'left'; ctx.fillText('CLASSIFIED // ARCHIVAL SCAN', 8*s, 13*s);
    ctx.textAlign = 'right'; ctx.fillText('STORM ACTIVE', w-8*s, 13*s);
    ctx.textAlign = 'left';

    // Name watermark
    ctx.fillStyle = tClr(p,0.35); ctx.font = '300 italic '+(20*s)+'px Cormorant Garamond, serif';
    ctx.textAlign = 'center'; ctx.fillText('Lance W. Fisher', w/2, h*0.38);
    // Subtitle
    ctx.fillStyle = tClr(p,0.12); ctx.font = '200 '+(7*s)+'px Inter, sans-serif';
    ctx.fillText('The Original Concept', w/2, h*0.45);
    ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p,0.15));
  }

  // ---- BOOKMARK BOT: CLI pipeline visualization ----
  function drawBookmarkBot(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    drawBg(ctx, w, h, 64, 180, 140);
    var p = pal(64, 180, 140);

    // Subtle circuit-board grid pattern
    ctx.strokeStyle = aClr(p,0.02); ctx.lineWidth = 0.5*s;
    for (var gi = 0; gi < w; gi += 24*s) { ctx.beginPath(); ctx.moveTo(gi, 0); ctx.lineTo(gi, h); ctx.stroke(); }
    for (var gj = 0; gj < h; gj += 24*s) { ctx.beginPath(); ctx.moveTo(0, gj); ctx.lineTo(w, gj); ctx.stroke(); }

    // Header bar with gradient
    var hGrad = ctx.createLinearGradient(0, 0, w, 0);
    hGrad.addColorStop(0, aClr(p,0.08)); hGrad.addColorStop(1, aClr(p,0.02));
    ctx.fillStyle = hGrad; ctx.fillRect(0, 0, w, 28*s);
    ctx.strokeStyle = aClr(p,0.1); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 28*s); ctx.lineTo(w, 28*s); ctx.stroke();
    ctx.fillStyle = aClr(p,0.6); ctx.fillRect(0, 0, 3*s, 28*s);

    // X logo stylized
    ctx.save();
    ctx.fillStyle = tClr(p,0.08); ctx.font = 'bold '+(40*s)+'px Inter, sans-serif';
    ctx.textAlign = 'right'; ctx.fillText('X', w-8*s, 60*s); ctx.restore();

    ctx.fillStyle = tClr(p,0.75); ctx.font = '500 '+(9*s)+'px Inter, sans-serif';
    ctx.textAlign = 'left'; ctx.fillText('Bookmark-to-Build Bot', 14*s, 18*s);
    ctx.fillStyle = 'rgba(74,222,128,0.5)'; ctx.font = '200 '+(6*s)+'px Inter, sans-serif';
    ctx.fillText('\u25CF  PIPELINE ACTIVE', w*0.55, 18*s);

    // Left panel: Bookmark feed (scrollable list mockup)
    var feedX = 6*s, feedY = 34*s, feedW = w*0.38, feedH = h*0.55;
    ctx.fillStyle = aClr(p,0.02); ctx.fillRect(feedX, feedY, feedW, feedH);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s; ctx.strokeRect(feedX, feedY, feedW, feedH);
    ctx.fillStyle = aClr(p,0.35); ctx.font = '200 '+(5.5*s)+'px Inter, sans-serif';
    ctx.fillText('BOOKMARK QUEUE', feedX+8*s, feedY+12*s);
    ctx.fillStyle = tClr(p,0.15); ctx.font = (5*s)+'px monospace';
    ctx.textAlign = 'right'; ctx.fillText('47 pending', feedX+feedW-6*s, feedY+12*s); ctx.textAlign = 'left';

    var bookmarks = [
      {user:'@levelsio', text:'Ship fast, iterate faster. Here\'s my stack...', tag:'patterns'},
      {user:'@thdxr', text:'SST Ion architecture for serverless TypeScript', tag:'arch'},
      {user:'@dan_abramov', text:'React compiler deep dive and mental model', tag:'react'},
      {user:'@karpathy', text:'Tokenization is all you need. Thread on BPE...', tag:'ai'},
      {user:'@naval', text:'Leverage and judgment scale. Code as leverage...', tag:'philosophy'},
      {user:'@swyx', text:'The rising tide of AI coding assistants', tag:'tools'}
    ];
    var tagColors = {patterns:'rgba(74,222,128,', arch:'rgba(96,165,250,', react:'rgba(147,130,220,', ai:'rgba(251,191,36,', philosophy:'rgba(240,120,120,', tools:'rgba(64,180,140,'};
    for (var bi = 0; bi < bookmarks.length; bi++) {
      var by = feedY+20*s + bi*18*s;
      if (by + 16*s > feedY + feedH) break;
      // Row separator
      ctx.strokeStyle = aClr(p,0.03); ctx.beginPath(); ctx.moveTo(feedX+6*s, by+16*s); ctx.lineTo(feedX+feedW-6*s, by+16*s); ctx.stroke();
      // Username
      ctx.fillStyle = aClr(p,0.5); ctx.font = '500 '+(5*s)+'px Inter, sans-serif';
      ctx.fillText(bookmarks[bi].user, feedX+8*s, by+6*s);
      // Text preview
      ctx.fillStyle = tClr(p,0.25); ctx.font = '300 '+(5*s)+'px Inter, sans-serif';
      var txt = bookmarks[bi].text; if (txt.length > 32) txt = txt.substring(0,30)+'...';
      ctx.fillText(txt, feedX+8*s, by+14*s);
      // Tag
      var tc = tagColors[bookmarks[bi].tag] || 'rgba(64,180,140,';
      ctx.fillStyle = tc+'0.1)'; ctx.fillRect(feedX+feedW-32*s, by+2*s, 26*s, 8*s);
      ctx.fillStyle = tc+'0.5)'; ctx.font = (4*s)+'px monospace'; ctx.fillText(bookmarks[bi].tag, feedX+feedW-30*s, by+8*s);
    }

    // Right side: Pipeline visualization (vertical flow)
    var pipeX = feedX+feedW+10*s, pipeY = 34*s;
    ctx.fillStyle = tClr(p,0.5); ctx.font = '300 '+(14*s)+'px Cormorant Garamond, serif';
    ctx.fillText('Pipeline', pipeX, pipeY+14*s);
    ctx.fillStyle = aClr(p,0.3); ctx.font = '200 '+(7*s)+'px Inter, sans-serif';
    ctx.fillText('5-stage processor', pipeX, pipeY+26*s);

    // Thin rule
    ctx.fillStyle = aClr(p,0.15); ctx.fillRect(pipeX, pipeY+30*s, 40*s, 0.5*s);

    var stages = [
      {name:'INGEST', desc:'Fetch bookmarks via API', status:'done', c:'rgba(74,222,128,'},
      {name:'EXTRACT', desc:'Parse ideas & patterns', status:'done', c:'rgba(74,222,128,'},
      {name:'MATCH', desc:'Map to codebase projects', status:'active', c:'rgba(64,180,140,'},
      {name:'PROPOSE', desc:'Generate PR descriptions', status:'pending', c:'rgba(64,180,140,'},
      {name:'IMPLEMENT', desc:'Apply with approval gate', status:'pending', c:'rgba(64,180,140,'}
    ];
    var stY = pipeY+40*s;
    for (var si = 0; si < stages.length; si++) {
      var sy = stY + si * 22*s;
      var isDone = stages[si].status === 'done';
      var isActive = stages[si].status === 'active';
      var isPending = stages[si].status === 'pending';

      // Vertical connection line
      if (si < stages.length - 1) {
        ctx.strokeStyle = isDone ? 'rgba(74,222,128,0.15)' : aClr(p,0.05);
        ctx.lineWidth = 1*s; ctx.setLineDash(isDone ? [] : [2*s, 3*s]);
        ctx.beginPath(); ctx.moveTo(pipeX+8*s, sy+12*s); ctx.lineTo(pipeX+8*s, sy+22*s); ctx.stroke();
        ctx.setLineDash([]);
      }

      // Status icon
      ctx.beginPath(); ctx.arc(pipeX+8*s, sy+4*s, 5*s, 0, Math.PI*2);
      ctx.fillStyle = isDone ? 'rgba(74,222,128,0.12)' : isActive ? aClr(p,0.15) : aClr(p,0.04);
      ctx.fill();
      ctx.strokeStyle = isDone ? 'rgba(74,222,128,0.4)' : isActive ? aClr(p,0.4) : aClr(p,0.08);
      ctx.lineWidth = isActive ? 1.5*s : 0.8*s; ctx.stroke();

      if (isDone) {
        ctx.fillStyle = 'rgba(74,222,128,0.8)'; ctx.font = 'bold '+(7*s)+'px Inter'; ctx.textAlign = 'center';
        ctx.fillText('\u2713', pipeX+8*s, sy+7*s); ctx.textAlign = 'left';
      } else if (isActive) {
        // Pulsing dot
        ctx.beginPath(); ctx.arc(pipeX+8*s, sy+4*s, 2*s, 0, Math.PI*2);
        ctx.fillStyle = aClr(p,0.6); ctx.fill();
      }

      // Stage name and description
      ctx.fillStyle = isDone ? 'rgba(74,222,128,0.6)' : isActive ? aClr(p,0.6) : aClr(p,0.2);
      ctx.font = '600 '+(6*s)+'px Inter, sans-serif'; ctx.fillText(stages[si].name, pipeX+20*s, sy+7*s);
      ctx.fillStyle = isPending ? tClr(p,0.1) : tClr(p,0.25);
      ctx.font = '300 '+(5.5*s)+'px Inter, sans-serif'; ctx.fillText(stages[si].desc, pipeX+65*s, sy+7*s);
    }

    // Bottom panel: Matched ideas with confidence scores
    var bY = h*0.65;
    ctx.fillStyle = aClr(p,0.02); ctx.fillRect(6*s, bY, w-12*s, h-bY-8*s);
    ctx.strokeStyle = aClr(p,0.05); ctx.lineWidth = 0.5*s; ctx.strokeRect(6*s, bY, w-12*s, h-bY-8*s);
    ctx.fillStyle = aClr(p,0.35); ctx.font = '200 '+(5.5*s)+'px Inter, sans-serif';
    ctx.fillText('MATCHED TO PROJECTS', 14*s, bY+12*s);

    var ideas = [
      {project:'master-trade-bot', idea:'Trailing stop-loss from @levelsio pattern', conf:92, c:'#4ade80'},
      {project:'home-hub', idea:'Motion detection alerts from @karpathy thread', conf:87, c:'#60a5fa'},
      {project:'medspa-booking', idea:'Waitlist feature from @spa_tech bookmark', conf:81, c:'#fbbf24'},
      {project:'profit-desk', idea:'Risk parity rebalancing from @naval thread', conf:74, c:'#c084fc'}
    ];
    for (var ii = 0; ii < ideas.length; ii++) {
      var iy = bY + 20*s + ii * 14*s;
      if (iy + 10*s > h - 12*s) break;
      // Confidence bar background
      ctx.fillStyle = aClr(p,0.03); ctx.fillRect(14*s, iy, w-28*s, 10*s);
      // Confidence bar fill
      ctx.fillStyle = aClr(p,0.08); ctx.fillRect(14*s, iy, (w-28*s)*ideas[ii].conf/100, 10*s);
      // Project name
      ctx.fillStyle = tClr(p,0.45); ctx.font = '500 '+(5*s)+'px monospace';
      ctx.fillText(ideas[ii].project, 20*s, iy+7*s);
      // Idea text
      ctx.fillStyle = tClr(p,0.2); ctx.font = '300 '+(5*s)+'px Inter, sans-serif';
      ctx.fillText(ideas[ii].idea, 120*s, iy+7*s);
      // Confidence percentage
      ctx.fillStyle = ideas[ii].c; ctx.globalAlpha = 0.6; ctx.font = '500 '+(5.5*s)+'px monospace';
      ctx.textAlign = 'right'; ctx.fillText(ideas[ii].conf+'%', w-16*s, iy+7*s);
      ctx.textAlign = 'left'; ctx.globalAlpha = 1;
    }

    drawCorners(ctx, w, h, 14*s, aClr(p,0.15));
  }

  // ---- HOME HUB: Security dashboard ----
  function drawHomeHub(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    drawBg(ctx, w, h, 60, 140, 220);
    var p = pal(60, 140, 220);

    // Subtle hex grid pattern (security aesthetic)
    ctx.strokeStyle = aClr(p,0.015); ctx.lineWidth = 0.5*s;
    var hexR = 18*s;
    for (var hx = 0; hx < w + hexR*2; hx += hexR*1.5) {
      for (var hy = 0; hy < h + hexR*2; hy += hexR*1.73) {
        var ox = (Math.floor(hy/(hexR*1.73)) % 2) * hexR*0.75;
        ctx.beginPath();
        for (var hi = 0; hi < 6; hi++) {
          var ha = Math.PI/3*hi - Math.PI/6;
          var hpx = hx+ox + hexR*0.5*Math.cos(ha), hpy = hy + hexR*0.5*Math.sin(ha);
          if (hi === 0) ctx.moveTo(hpx, hpy); else ctx.lineTo(hpx, hpy);
        }
        ctx.closePath(); ctx.stroke();
      }
    }

    // Header bar with shield icon effect
    var hGrad = ctx.createLinearGradient(0, 0, w, 0);
    hGrad.addColorStop(0, aClr(p,0.08)); hGrad.addColorStop(1, aClr(p,0.02));
    ctx.fillStyle = hGrad; ctx.fillRect(0, 0, w, 28*s);
    ctx.strokeStyle = aClr(p,0.1); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 28*s); ctx.lineTo(w, 28*s); ctx.stroke();
    ctx.fillStyle = aClr(p,0.6); ctx.fillRect(0, 0, 3*s, 28*s);

    // Shield icon
    ctx.save();
    ctx.beginPath(); ctx.moveTo(10*s, 8*s); ctx.lineTo(18*s, 5*s); ctx.lineTo(26*s, 8*s);
    ctx.lineTo(26*s, 16*s); ctx.quadraticCurveTo(18*s, 24*s, 18*s, 24*s);
    ctx.quadraticCurveTo(18*s, 24*s, 10*s, 16*s); ctx.closePath();
    ctx.strokeStyle = aClr(p,0.35); ctx.lineWidth = 1*s; ctx.stroke();
    ctx.fillStyle = aClr(p,0.05); ctx.fill();
    ctx.restore();

    ctx.fillStyle = tClr(p,0.75); ctx.font = '500 '+(9*s)+'px Inter, sans-serif';
    ctx.textAlign = 'left'; ctx.fillText('Home Hub', 32*s, 18*s);
    // Status
    ctx.beginPath(); ctx.arc(w-14*s, 14*s, 3*s, 0, Math.PI*2);
    ctx.fillStyle = '#4ade80'; ctx.fill();
    ctx.fillStyle = 'rgba(74,222,128,0.5)'; ctx.font = (6*s)+'px monospace';
    ctx.textAlign = 'right'; ctx.fillText('SECURE', w-22*s, 17*s); ctx.textAlign = 'left';

    // Tab strip (7-tab dashboard nav)
    var hhTabsY = 28*s, hhTabsH = 14*s;
    ctx.fillStyle = aClr(p,0.025); ctx.fillRect(0, hhTabsY, w, hhTabsH);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, hhTabsY+hhTabsH); ctx.lineTo(w, hhTabsY+hhTabsH); ctx.stroke();
    var hhTabs = ['Devices','Cameras','Network','Alerts','Bandwidth','Logs','Settings'];
    var hhTabW = w / hhTabs.length;
    for (var hht = 0; hht < hhTabs.length; hht++) {
      var hhtx = hht*hhTabW;
      if (hht === 1) {
        ctx.fillStyle = aClr(p,0.08); ctx.fillRect(hhtx, hhTabsY, hhTabW, hhTabsH);
        ctx.fillStyle = aClr(p,0.7); ctx.fillRect(hhtx, hhTabsY+hhTabsH-1.5*s, hhTabW, 1.5*s);
      }
      ctx.fillStyle = hht === 1 ? tClr(p,0.6) : tClr(p,0.25);
      ctx.font = (hht === 1 ? '500 ' : '300 ')+(5.5*s)+'px Inter, sans-serif';
      ctx.textAlign = 'center'; ctx.fillText(hhTabs[hht], hhtx+hhTabW/2, hhTabsY+9.5*s);
    }
    ctx.textAlign = 'left';

    // Left panel: Camera feeds (2x2)
    var camX = 6*s, camY = 48*s, camW = w*0.44, camH = h*0.48;
    ctx.fillStyle = aClr(p,0.02); ctx.fillRect(camX, camY, camW, camH);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s; ctx.strokeRect(camX, camY, camW, camH);
    ctx.fillStyle = aClr(p,0.35); ctx.font = '200 '+(5.5*s)+'px Inter, sans-serif';
    ctx.fillText('CAMERA FEEDS', camX+8*s, camY+12*s);
    ctx.fillStyle = 'rgba(248,113,113,0.4)'; ctx.font = (4.5*s)+'px monospace';
    ctx.textAlign = 'right'; ctx.fillText('\u25CF REC', camX+camW-6*s, camY+12*s); ctx.textAlign = 'left';

    var feedW2 = (camW-18*s)/2, feedH2 = (camH-36*s)/2;
    var cameras = [
      {name:'Front Door', status:'online', motion:true},
      {name:'Backyard', status:'online', motion:false},
      {name:'Garage', status:'online', motion:false},
      {name:'Driveway', status:'offline', motion:false}
    ];
    for (var ci = 0; ci < 4; ci++) {
      var fx = camX+6*s + (ci%2)*(feedW2+6*s);
      var fy = camY+20*s + Math.floor(ci/2)*(feedH2+6*s);
      var isOnline = cameras[ci].status === 'online';

      // Feed background with slight noise texture
      ctx.fillStyle = isOnline ? aClr(p,0.04) : 'rgba(200,60,60,0.03)';
      ctx.fillRect(fx, fy, feedW2, feedH2);
      ctx.strokeStyle = isOnline ? aClr(p,0.1) : 'rgba(200,60,60,0.1)';
      ctx.lineWidth = 0.5*s; ctx.strokeRect(fx, fy, feedW2, feedH2);

      // Camera name badge
      ctx.fillStyle = (p.light ? 'rgba(26,26,28,0.12)' : 'rgba(0,0,0,0.3)'); ctx.fillRect(fx+2*s, fy+2*s, 40*s, 9*s);
      ctx.fillStyle = tClr(p,0.5); ctx.font = '500 '+(4.5*s)+'px Inter, sans-serif';
      ctx.fillText(cameras[ci].name, fx+4*s, fy+9*s);

      // Status dot
      ctx.beginPath(); ctx.arc(fx+feedW2-6*s, fy+6*s, 2.5*s, 0, Math.PI*2);
      ctx.fillStyle = isOnline ? '#4ade80' : '#f87171'; ctx.fill();

      if (isOnline) {
        // Scanlines for video feel
        for (var sl = 0; sl < feedH2; sl += 3*s) {
          ctx.fillStyle = aClr(p,0.01); ctx.fillRect(fx, fy+sl, feedW2, 1*s);
        }
        // Motion detection box
        if (cameras[ci].motion) {
          ctx.strokeStyle = 'rgba(74,222,128,0.3)'; ctx.lineWidth = 0.8*s;
          ctx.setLineDash([2*s, 2*s]);
          ctx.strokeRect(fx+feedW2*0.3, fy+feedH2*0.3, feedW2*0.4, feedH2*0.4);
          ctx.setLineDash([]);
          ctx.fillStyle = 'rgba(74,222,128,0.4)'; ctx.font = (3.5*s)+'px monospace';
          ctx.fillText('MOTION', fx+feedW2*0.32, fy+feedH2*0.28);
        }
        // Timestamp
        ctx.fillStyle = tClr(p,0.2); ctx.font = (3.5*s)+'px monospace';
        ctx.fillText('02:41:33', fx+4*s, fy+feedH2-4*s);
      } else {
        // Offline X
        ctx.strokeStyle = 'rgba(248,113,113,0.15)'; ctx.lineWidth = 1.5*s;
        ctx.beginPath(); ctx.moveTo(fx+feedW2*0.35, fy+feedH2*0.35); ctx.lineTo(fx+feedW2*0.65, fy+feedH2*0.65); ctx.stroke();
        ctx.beginPath(); ctx.moveTo(fx+feedW2*0.65, fy+feedH2*0.35); ctx.lineTo(fx+feedW2*0.35, fy+feedH2*0.65); ctx.stroke();
        ctx.fillStyle = 'rgba(248,113,113,0.3)'; ctx.font = (4*s)+'px monospace';
        ctx.textAlign = 'center'; ctx.fillText('OFFLINE', fx+feedW2/2, fy+feedH2*0.8); ctx.textAlign = 'left';
      }
    }

    // Right panel: Network topology
    var netX = camX+camW+6*s, netY = 48*s, netW = w-netX-6*s, netH = h*0.48;
    ctx.fillStyle = aClr(p,0.02); ctx.fillRect(netX, netY, netW, netH);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s; ctx.strokeRect(netX, netY, netW, netH);
    ctx.fillStyle = aClr(p,0.35); ctx.font = '200 '+(5.5*s)+'px Inter, sans-serif';
    ctx.fillText('NETWORK TOPOLOGY', netX+8*s, netY+12*s);
    ctx.fillStyle = tClr(p,0.2); ctx.font = (5*s)+'px monospace';
    ctx.textAlign = 'right'; ctx.fillText('14 devices', netX+netW-8*s, netY+12*s); ctx.textAlign = 'left';

    // Router (central hub with rings)
    var routerX = netX+netW*0.5, routerY = netY+netH*0.5;
    // Outer ring
    ctx.beginPath(); ctx.arc(routerX, routerY, 20*s, 0, Math.PI*2);
    ctx.strokeStyle = aClr(p,0.05); ctx.lineWidth = 0.5*s; ctx.stroke();
    // Inner ring
    ctx.beginPath(); ctx.arc(routerX, routerY, 12*s, 0, Math.PI*2);
    ctx.fillStyle = aClr(p,0.08); ctx.fill();
    ctx.strokeStyle = aClr(p,0.25); ctx.lineWidth = 1.2*s; ctx.stroke();
    ctx.fillStyle = tClr(p,0.6); ctx.font = '600 '+(6*s)+'px Inter, sans-serif';
    ctx.textAlign = 'center'; ctx.fillText('GW', routerX, routerY+2*s);

    // Device nodes with connection lines
    var devices = [
      {x:0.18, y:0.3, label:'PC', c:'#4ade80'},
      {x:0.28, y:0.75, label:'TV', c:'#fbbf24'},
      {x:0.82, y:0.28, label:'NAS', c:'#4ade80'},
      {x:0.78, y:0.7, label:'IoT', c:'#fbbf24'},
      {x:0.12, y:0.6, label:'CAM', c:'#60a5fa'},
      {x:0.88, y:0.5, label:'???', c:'#f87171'},
      {x:0.4, y:0.2, label:'PH', c:'#4ade80'},
      {x:0.6, y:0.8, label:'CAM', c:'#60a5fa'},
      {x:0.7, y:0.2, label:'LAP', c:'#4ade80'},
      {x:0.35, y:0.55, label:'TAB', c:'#4ade80'}
    ];
    for (var di = 0; di < devices.length; di++) {
      var dx = netX+netW*devices[di].x, dy = netY+20*s+(netH-32*s)*devices[di].y;
      // Connection line with gradient
      ctx.strokeStyle = devices[di].c === '#f87171' ? 'rgba(248,113,113,0.08)' : aClr(p,0.06);
      ctx.lineWidth = 0.5*s; ctx.beginPath(); ctx.moveTo(routerX, routerY); ctx.lineTo(dx, dy); ctx.stroke();
      // Device circle
      ctx.beginPath(); ctx.arc(dx, dy, 5*s, 0, Math.PI*2);
      ctx.fillStyle = devices[di].c; ctx.globalAlpha = 0.15; ctx.fill(); ctx.globalAlpha = 1;
      ctx.strokeStyle = devices[di].c; ctx.globalAlpha = 0.4; ctx.lineWidth = 0.8*s; ctx.stroke(); ctx.globalAlpha = 1;
      // Label
      ctx.fillStyle = devices[di].c; ctx.globalAlpha = 0.4;
      ctx.font = (3.5*s)+'px monospace'; ctx.textAlign = 'center'; ctx.fillText(devices[di].label, dx, dy+10*s);
      ctx.globalAlpha = 1;
    }
    ctx.textAlign = 'left';

    // Bottom panel: Alerts with severity indicators
    var alertY = Math.max(camY+camH, netY+netH) + 6*s;
    var alertH = h-alertY-6*s;
    ctx.fillStyle = aClr(p,0.02); ctx.fillRect(6*s, alertY, w-12*s, alertH);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s; ctx.strokeRect(6*s, alertY, w-12*s, alertH);
    ctx.fillStyle = aClr(p,0.35); ctx.font = '200 '+(5.5*s)+'px Inter, sans-serif';
    ctx.fillText('ALERTS & EVENTS', 14*s, alertY+12*s);

    // Stats bar
    var stats = [{v:'14', l:'Devices', c:'#4ade80'}, {v:'3', l:'Cameras', c:'#60a5fa'}, {v:'1.2GB', l:'/hr', c:'#fbbf24'}, {v:'1', l:'Warning', c:'#f87171'}];
    var stW2 = (w-24*s)/4;
    for (var sti = 0; sti < stats.length; sti++) {
      var stx = 12*s + sti*stW2;
      ctx.fillStyle = stats[sti].c; ctx.globalAlpha = 0.6;
      ctx.font = '600 '+(8*s)+'px Inter, sans-serif'; ctx.fillText(stats[sti].v, stx, alertY+26*s);
      ctx.globalAlpha = 0.3; ctx.font = (5*s)+'px Inter, sans-serif'; ctx.fillText(stats[sti].l, stx, alertY+34*s);
      ctx.globalAlpha = 1;
    }

    var alerts = [
      {type:'WARN', msg:'Unknown device joined: MAC aa:bb:cc:12:34', t:'2m ago', c:'#fbbf24'},
      {type:'DOWN', msg:'Driveway camera lost connection', t:'14m ago', c:'#f87171'},
      {type:'INFO', msg:'Smart TV bandwidth: 500MB/hr', t:'1h ago', c:'#60a5fa'}
    ];
    for (var ai = 0; ai < alerts.length; ai++) {
      var ay = alertY+42*s + ai*12*s;
      if (ay + 10*s > h - 8*s) break;
      // Severity bar
      ctx.fillStyle = alerts[ai].c; ctx.globalAlpha = 0.15;
      ctx.fillRect(14*s, ay-1*s, 2*s, 10*s); ctx.globalAlpha = 1;
      // Type badge
      ctx.fillStyle = alerts[ai].c; ctx.globalAlpha = 0.5;
      ctx.font = 'bold '+(4.5*s)+'px monospace'; ctx.fillText(alerts[ai].type, 22*s, ay+6*s);
      ctx.globalAlpha = 1;
      // Message
      ctx.fillStyle = tClr(p,0.3); ctx.font = '300 '+(5*s)+'px Inter, sans-serif';
      ctx.fillText(alerts[ai].msg, 50*s, ay+6*s);
      // Time
      ctx.fillStyle = tClr(p,0.12); ctx.font = (4.5*s)+'px monospace';
      ctx.textAlign = 'right'; ctx.fillText(alerts[ai].t, w-14*s, ay+6*s); ctx.textAlign = 'left';
    }

    drawCorners(ctx, w, h, 14*s, aClr(p,0.15));
  }

  // ===== PROJECT HUB =====
  function drawProjectHub(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    drawBg(ctx, w, h, 201, 168, 76);
    var p = pal(201, 168, 76);

    // Subtle grid pattern (command center)
    ctx.strokeStyle = aClr(p,0.02); ctx.lineWidth = 0.5*s;
    for (var gi = 0; gi < w; gi += 20*s) { ctx.beginPath(); ctx.moveTo(gi, 0); ctx.lineTo(gi, h); ctx.stroke(); }
    for (var gj = 0; gj < h; gj += 20*s) { ctx.beginPath(); ctx.moveTo(0, gj); ctx.lineTo(w, gj); ctx.stroke(); }

    // Header bar
    var hGrad = ctx.createLinearGradient(0, 0, w, 0);
    hGrad.addColorStop(0, aClr(p,0.08)); hGrad.addColorStop(1, aClr(p,0.02));
    ctx.fillStyle = hGrad; ctx.fillRect(0, 0, w, 28*s);
    ctx.strokeStyle = aClr(p,0.1); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 28*s); ctx.lineTo(w, 28*s); ctx.stroke();
    ctx.fillStyle = aClr(p,0.6); ctx.fillRect(0, 0, 3*s, 28*s);
    ctx.fillStyle = tClr(p,0.75); ctx.font = '500 '+(9*s)+'px Inter, sans-serif';
    ctx.textAlign = 'left'; ctx.fillText('Sovereign Hub', 14*s, 18*s);

    // Sync status
    ctx.beginPath(); ctx.arc(w-14*s, 14*s, 3*s, 0, Math.PI*2);
    ctx.fillStyle = '#4ade80'; ctx.fill();
    ctx.fillStyle = 'rgba(74,222,128,0.5)'; ctx.font = (6*s)+'px monospace';
    ctx.textAlign = 'right'; ctx.fillText('SYNCED', w-22*s, 17*s); ctx.textAlign = 'left';

    // Tab strip (7-tab command surface)
    var tabsY = 28*s, tabsH = 14*s;
    ctx.fillStyle = aClr(p,0.025); ctx.fillRect(0, tabsY, w, tabsH);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, tabsY+tabsH); ctx.lineTo(w, tabsY+tabsH); ctx.stroke();
    var hubTabs = ['Overview','Repos','Sync','Activity','Logs','Health','Settings'];
    var hubTabW = w / hubTabs.length;
    for (var ht = 0; ht < hubTabs.length; ht++) {
      var htx = ht*hubTabW;
      if (ht === 0) {
        // Active tab indicator
        ctx.fillStyle = aClr(p,0.08); ctx.fillRect(htx, tabsY, hubTabW, tabsH);
        ctx.fillStyle = aClr(p,0.7); ctx.fillRect(htx, tabsY+tabsH-1.5*s, hubTabW, 1.5*s);
      }
      ctx.fillStyle = ht === 0 ? tClr(p,0.6) : tClr(p,0.25);
      ctx.font = (ht === 0 ? '500 ' : '300 ')+(5.5*s)+'px Inter, sans-serif';
      ctx.textAlign = 'center'; ctx.fillText(hubTabs[ht], htx+hubTabW/2, tabsY+9.5*s);
    }
    ctx.textAlign = 'left';

    // Left side: Project list table
    var tableX = 6*s, tableY = 48*s, tableW = w*0.55, tableH = h - 84*s;
    ctx.fillStyle = aClr(p,0.02); ctx.fillRect(tableX, tableY, tableW, tableH);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s; ctx.strokeRect(tableX, tableY, tableW, tableH);

    // Table header
    ctx.fillStyle = aClr(p,0.06); ctx.fillRect(tableX, tableY, tableW, 12*s);
    ctx.fillStyle = aClr(p,0.4); ctx.font = '500 '+(4.5*s)+'px Inter, sans-serif';
    ctx.fillText('PROJECT', tableX+22*s, tableY+8*s);
    ctx.fillText('STATUS', tableX+tableW*0.62, tableY+8*s);
    ctx.fillText('LANG', tableX+tableW*0.82, tableY+8*s);

    var projects = [
      {name:'medspa-booking', status:'clean', lang:'TS/RN', commits:'+3'},
      {name:'profit-desk', status:'dirty', lang:'PY', commits:'+12'},
      {name:'master-trade-bot', status:'ahead', lang:'TS', commits:'+8'},
      {name:'frontier-bastion', status:'clean', lang:'TS', commits:'+1'},
      {name:'e2ee-messenger', status:'clean', lang:'TS', commits:'+5'},
      {name:'polymarket-btc', status:'clean', lang:'JS', commits:'+2'},
      {name:'JumpQuest', status:'dirty', lang:'C#', commits:'+7'},
      {name:'home-hub', status:'clean', lang:'TS', commits:'+4'},
      {name:'auton', status:'clean', lang:'PY', commits:'+6'},
      {name:'x-bookmark-bot', status:'clean', lang:'PY', commits:'+3'}
    ];
    var statusColors = {clean:'#4ade80', dirty:'#fbbf24', ahead:'#60a5fa'};
    var rowH = Math.min(16*s, (tableH-14*s)/projects.length);
    for (var pi = 0; pi < projects.length; pi++) {
      var py = tableY+14*s + pi*rowH;
      if (py + rowH > tableY + tableH) break;
      var proj = projects[pi];

      // Alternating row shade
      if (pi % 2 === 0) { ctx.fillStyle = tClr(p,0.008); ctx.fillRect(tableX, py, tableW, rowH); }

      // Status dot
      ctx.beginPath(); ctx.arc(tableX+10*s, py+rowH/2, 2*s, 0, Math.PI*2);
      ctx.fillStyle = statusColors[proj.status]; ctx.globalAlpha = 0.5; ctx.fill(); ctx.globalAlpha = 1;

      // Project name
      ctx.fillStyle = tClr(p,0.4); ctx.font = (6*s)+'px Inter, sans-serif';
      ctx.fillText(proj.name, tableX+20*s, py+rowH/2+2*s);

      // Status text
      ctx.fillStyle = statusColors[proj.status]; ctx.globalAlpha = 0.5;
      ctx.font = (5*s)+'px monospace'; ctx.fillText(proj.status.toUpperCase(), tableX+tableW*0.62, py+rowH/2+2*s);
      ctx.globalAlpha = 1;

      // Language badge
      ctx.fillStyle = aClr(p,0.08); ctx.fillRect(tableX+tableW*0.82, py+2*s, 20*s, rowH-4*s);
      ctx.fillStyle = aClr(p,0.45); ctx.font = (5*s)+'px monospace';
      ctx.fillText(proj.lang, tableX+tableW*0.84, py+rowH/2+2*s);

      // Commit count
      ctx.fillStyle = 'rgba(74,222,128,0.3)'; ctx.font = (4.5*s)+'px monospace';
      ctx.textAlign = 'right'; ctx.fillText(proj.commits, tableX+tableW-6*s, py+rowH/2+2*s); ctx.textAlign = 'left';
    }

    // Right side: Stats and activity
    var rX = tableX+tableW+8*s, rY = 48*s, rW = w-rX-6*s;

    // Big stat cards
    var statCards = [
      {v:'19', l:'Repos', c:'#4ade80'},
      {v:'6', l:'Active', c:'#60a5fa'},
      {v:'2', l:'Dirty', c:'#fbbf24'},
      {v:'47', l:'Commits', c:'#c9a84c'}
    ];
    var scW = (rW-6*s)/2, scH = 28*s;
    for (var sc = 0; sc < 4; sc++) {
      var scx = rX + (sc%2)*(scW+6*s);
      var scy = rY + Math.floor(sc/2)*(scH+6*s);
      ctx.fillStyle = aClr(p,0.03); ctx.fillRect(scx, scy, scW, scH);
      ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s; ctx.strokeRect(scx, scy, scW, scH);
      ctx.fillStyle = statCards[sc].c; ctx.globalAlpha = 0.7;
      ctx.font = '600 '+(14*s)+'px Inter, sans-serif'; ctx.fillText(statCards[sc].v, scx+6*s, scy+18*s);
      ctx.globalAlpha = 0.35; ctx.font = (5.5*s)+'px Inter, sans-serif'; ctx.fillText(statCards[sc].l, scx+6*s, scy+25*s);
      ctx.globalAlpha = 1;
    }

    // Activity graph (sparkline)
    var graphY = rY + 2*(scH+6*s) + 6*s;
    var graphH = h - graphY - 34*s;
    ctx.fillStyle = aClr(p,0.02); ctx.fillRect(rX, graphY, rW, graphH);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s; ctx.strokeRect(rX, graphY, rW, graphH);
    ctx.fillStyle = aClr(p,0.35); ctx.font = '200 '+(5*s)+'px Inter, sans-serif';
    ctx.fillText('COMMIT ACTIVITY', rX+6*s, graphY+10*s);

    // Draw sparkline bars (last 14 days)
    var days = [3,7,2,5,12,8,4,6,9,3,11,5,7,4];
    var barW2 = (rW-24*s)/days.length;
    var maxD = 12;
    for (var di = 0; di < days.length; di++) {
      var bx = rX+10*s + di*(barW2+1*s);
      var bh = (days[di]/maxD)*(graphH-20*s);
      var bGrad = ctx.createLinearGradient(0, graphY+graphH-6*s-bh, 0, graphY+graphH-6*s);
      bGrad.addColorStop(0, aClr(p,0.3)); bGrad.addColorStop(1, aClr(p,0.05));
      ctx.fillStyle = bGrad; ctx.fillRect(bx, graphY+graphH-6*s-bh, barW2, bh);
    }

    // Bottom bar: sync info
    var barY = h-24*s;
    ctx.fillStyle = aClr(p,0.04); ctx.fillRect(0, barY, w, 24*s);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, barY); ctx.lineTo(w, barY); ctx.stroke();
    ctx.fillStyle = tClr(p,0.25); ctx.font = (5.5*s)+'px Inter, sans-serif';
    ctx.fillText('Last sync: 2m ago', 14*s, barY+15*s);
    ctx.fillStyle = 'rgba(74,222,128,0.3)'; ctx.font = (5.5*s)+'px monospace';
    ctx.textAlign = 'center'; ctx.fillText('5-min auto-sync active', w/2, barY+15*s);
    ctx.fillStyle = aClr(p,0.25); ctx.font = (5.5*s)+'px Inter, sans-serif';
    ctx.textAlign = 'right'; ctx.fillText('manifest v2.4', w-14*s, barY+15*s);
    ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p,0.15));
  }

  // ---- FISHER ONE-THREE: Nautical brand showcase ----
  function drawFisher13(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(140, 170, 210);

    // Deep ocean night sky
    var sky = ctx.createLinearGradient(0, 0, 0, h);
    if (p.light) { sky.addColorStop(0, '#dfe5ed'); sky.addColorStop(0.35, '#d8dee8'); sky.addColorStop(0.7, '#d2d9e4'); sky.addColorStop(1, '#dce2ec'); }
    else { sky.addColorStop(0, '#050810'); sky.addColorStop(0.35, '#0a1020'); sky.addColorStop(0.7, '#0c1428'); sky.addColorStop(1, '#060a14'); }
    ctx.fillStyle = sky; ctx.fillRect(0, 0, w, h);

    // Subtle ocean waves pattern at bottom
    var oceanY = h * 0.72;
    var ocean = ctx.createLinearGradient(0, oceanY, 0, h);
    ocean.addColorStop(0, p.light ? 'rgba(140,170,210,0.0)' : 'rgba(15,30,60,0.0)');
    ocean.addColorStop(0.3, p.light ? 'rgba(140,170,210,0.08)' : 'rgba(15,30,60,0.2)');
    ocean.addColorStop(1, p.light ? 'rgba(140,170,210,0.15)' : 'rgba(8,16,32,0.6)');
    ctx.fillStyle = ocean; ctx.fillRect(0, oceanY, w, h - oceanY);

    // Animated-style wave lines
    ctx.strokeStyle = aClr(p,0.04); ctx.lineWidth = 0.6*s;
    for (var wv = 0; wv < 6; wv++) {
      ctx.beginPath();
      for (var wx = 0; wx < w; wx += 2*s) {
        var wy = oceanY + 12*s + wv*8*s + Math.sin(wx/(18*s) + wv*1.8)*3*s;
        if (wx === 0) ctx.moveTo(wx, wy); else ctx.lineTo(wx, wy);
      }
      ctx.stroke();
    }

    // Star field
    for (var si = 0; si < 35; si++) {
      var sx = (Math.sin(si*7.3+2.1)*0.5+0.5)*w;
      var sy = (Math.sin(si*4.1+0.7)*0.3+0.05)*h;
      var sa = 0.06 + Math.sin(si*3.7)*0.04;
      ctx.fillStyle = p.light ? 'rgba(100,115,140,'+sa+')' : 'rgba(200,215,240,'+sa+')';
      ctx.fillRect(sx, sy, 1*s, 1*s);
    }

    // Compass rose (subtle background element)
    ctx.save();
    ctx.translate(w*0.82, h*0.25);
    ctx.globalAlpha = 0.035;
    ctx.strokeStyle = p.light ? '#6080a0' : '#8caad2'; ctx.lineWidth = 1*s;
    var cr = 30*s;
    ctx.beginPath(); ctx.arc(0, 0, cr, 0, Math.PI*2); ctx.stroke();
    ctx.beginPath(); ctx.arc(0, 0, cr*0.7, 0, Math.PI*2); ctx.stroke();
    // N-S-E-W lines
    for (var ci = 0; ci < 8; ci++) {
      var ca = ci*Math.PI/4;
      ctx.beginPath(); ctx.moveTo(0, 0);
      ctx.lineTo(Math.cos(ca)*cr*1.15, Math.sin(ca)*cr*1.15); ctx.stroke();
    }
    // Cardinal points
    ctx.fillStyle = p.light ? '#6080a0' : '#8caad2'; ctx.font = '600 '+(5*s)+'px Inter, sans-serif';
    ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
    ctx.fillText('N', 0, -cr*1.3);
    ctx.fillText('S', 0, cr*1.3);
    ctx.fillText('E', cr*1.3, 0);
    ctx.fillText('W', -cr*1.3, 0);
    ctx.restore();

    // Header bar with brand accent
    var hGrad = ctx.createLinearGradient(0, 0, w, 0);
    hGrad.addColorStop(0, aClr(p,0.06)); hGrad.addColorStop(1, aClr(p,0.01));
    ctx.fillStyle = hGrad; ctx.fillRect(0, 0, w, 24*s);
    ctx.strokeStyle = aClr(p,0.08); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 24*s); ctx.lineTo(w, 24*s); ctx.stroke();
    // Left accent bar
    ctx.fillStyle = aClr(p,0.4); ctx.fillRect(0, 0, 3*s, 24*s);

    // Header text
    ctx.fillStyle = tClr(p,0.65); ctx.font = '500 '+(8*s)+'px Inter, sans-serif';
    ctx.textAlign = 'left'; ctx.fillText('Fisher One-Three', 14*s, 16*s);
    ctx.fillStyle = aClr(p,0.4); ctx.font = '200 '+(6*s)+'px Inter, sans-serif';
    ctx.textAlign = 'right'; ctx.fillText('EST. 2024', w-14*s, 16*s);
    ctx.textAlign = 'left';

    // Load the brand crest image (winged anchor)
    loadImg('thumbs/fisher13-crest.jpg', function(img) {
      if (!img) return;
      // Center the crest image
      var maxW = w*0.48, maxH = h*0.52;
      var ratio = img.width / img.height;
      var iw, ih;
      if (ratio > maxW/maxH) { iw = maxW; ih = iw / ratio; }
      else { ih = maxH; iw = ih * ratio; }
      var ix = (w - iw)/2, iy = h*0.18;
      ctx.globalAlpha = 0.55;
      ctx.drawImage(img, ix, iy, iw, ih);
      ctx.globalAlpha = 1.0;
      // Vignette fade around image
      var vigTop = ctx.createLinearGradient(0, iy - 10*s, 0, iy + 20*s);
      vigTop.addColorStop(0, p.light ? 'rgba(223,229,237,1)' : 'rgba(5,8,16,1)'); vigTop.addColorStop(1, p.light ? 'rgba(223,229,237,0)' : 'rgba(5,8,16,0)');
      ctx.fillStyle = vigTop; ctx.fillRect(ix-5*s, iy-10*s, iw+10*s, 30*s);
      var vigBot = ctx.createLinearGradient(0, iy+ih-25*s, 0, iy+ih+5*s);
      vigBot.addColorStop(0, p.light ? 'rgba(223,229,237,0)' : 'rgba(5,8,16,0)'); vigBot.addColorStop(1, p.light ? 'rgba(223,229,237,0.9)' : 'rgba(5,8,16,0.9)');
      ctx.fillStyle = vigBot; ctx.fillRect(ix-5*s, iy+ih-25*s, iw+10*s, 30*s);
      // Re-draw corners after async
      drawCorners(ctx, w, h, 14*s, aClr(p,0.12));
    });

    // Brand tagline
    ctx.fillStyle = tClr(p,0.28); ctx.font = '300 italic '+(9*s)+'px Cormorant Garamond, serif';
    ctx.textAlign = 'center'; ctx.fillText('One Part Land, Three Parts Sea', w/2, h*0.78);

    // Bottom info bar
    var barY2 = h - 22*s;
    ctx.fillStyle = aClr(p,0.03); ctx.fillRect(0, barY2, w, 22*s);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, barY2); ctx.lineTo(w, barY2); ctx.stroke();

    // T-shirt size badges
    var sizes = ['XS','S','M','L','XL','2XL','3XL','4XL','5XL'];
    var badgeW = 18*s, gap2 = 3*s;
    var totalW = sizes.length*(badgeW+gap2) - gap2;
    var startX = (w - totalW)/2;
    for (var bi = 0; bi < sizes.length; bi++) {
      var bx2 = startX + bi*(badgeW+gap2);
      ctx.fillStyle = aClr(p,0.04); ctx.fillRect(bx2, barY2+4*s, badgeW, 14*s);
      ctx.strokeStyle = aClr(p,0.08); ctx.lineWidth = 0.5*s; ctx.strokeRect(bx2, barY2+4*s, badgeW, 14*s);
      ctx.fillStyle = tClr(p,0.3); ctx.font = '200 '+(4.5*s)+'px Inter, sans-serif';
      ctx.textAlign = 'center'; ctx.fillText(sizes[bi], bx2+badgeW/2, barY2+14*s);
    }
    ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p,0.12));
  }

  // ---- NOCO APP STUDIO: Phone mockup demos ----
  function drawNoCo(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(201, 168, 76);

    // Warm gradient
    var bg = ctx.createLinearGradient(0, 0, w*0.3, h);
    if (p.light) { bg.addColorStop(0, '#e8e5e0'); bg.addColorStop(0.5, '#e4e1dc'); bg.addColorStop(1, '#e6e3de'); }
    else { bg.addColorStop(0, '#0c0a08'); bg.addColorStop(0.5, '#0e0b09'); bg.addColorStop(1, '#0a0907'); }
    ctx.fillStyle = bg; ctx.fillRect(0, 0, w, h);

    // Subtle mountain silhouette (Northern Colorado)
    ctx.beginPath();
    ctx.moveTo(0, h*0.52);
    ctx.lineTo(w*0.12, h*0.38); ctx.lineTo(w*0.22, h*0.42);
    ctx.lineTo(w*0.35, h*0.28); ctx.lineTo(w*0.45, h*0.35);
    ctx.lineTo(w*0.55, h*0.25); ctx.lineTo(w*0.68, h*0.32);
    ctx.lineTo(w*0.78, h*0.3); ctx.lineTo(w*0.88, h*0.38);
    ctx.lineTo(w, h*0.42); ctx.lineTo(w, h); ctx.lineTo(0, h);
    ctx.closePath();
    ctx.fillStyle = aClr(p,0.012); ctx.fill();

    // Grid dots (app grid aesthetic)
    for (var gx = 0; gx < w; gx += 16*s) {
      for (var gy = 0; gy < h; gy += 16*s) {
        ctx.fillStyle = aClr(p,0.015);
        ctx.fillRect(gx, gy, 1*s, 1*s);
      }
    }

    // Header bar
    var hGrad = ctx.createLinearGradient(0, 0, w, 0);
    hGrad.addColorStop(0, aClr(p,0.06)); hGrad.addColorStop(1, aClr(p,0.01));
    ctx.fillStyle = hGrad; ctx.fillRect(0, 0, w, 24*s);
    ctx.strokeStyle = aClr(p,0.08); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 24*s); ctx.lineTo(w, 24*s); ctx.stroke();
    ctx.fillStyle = aClr(p,0.5); ctx.fillRect(0, 0, 3*s, 24*s);
    ctx.fillStyle = tClr(p,0.65); ctx.font = '500 '+(8*s)+'px Inter, sans-serif';
    ctx.fillText('NoCo App Studio', 14*s, 16*s);
    ctx.fillStyle = aClr(p,0.4); ctx.font = '200 '+(6*s)+'px Inter, sans-serif';
    ctx.textAlign = 'right'; ctx.fillText('10 CLIENTS', w-14*s, 16*s); ctx.textAlign = 'left';

    // Phone mockup (centered)
    var phoneW = 44*s, phoneH = 80*s;
    var phoneX = (w - phoneW)/2, phoneY = h*0.2;
    ctx.strokeStyle = aClr(p,0.18); ctx.lineWidth = 1.5*s;
    ctx.beginPath();
    ctx.roundRect(phoneX, phoneY, phoneW, phoneH, 4*s);
    ctx.stroke();
    // Screen
    ctx.fillStyle = aClr(p,0.02);
    ctx.beginPath(); ctx.roundRect(phoneX+3*s, phoneY+8*s, phoneW-6*s, phoneH-16*s, 2*s);
    ctx.fill();
    // Notch
    ctx.fillStyle = aClr(p,0.1);
    ctx.fillRect(phoneX + phoneW*0.3, phoneY+2*s, phoneW*0.4, 3*s);
    // Screen content - app list items
    var screenX = phoneX+6*s, screenW = phoneW-12*s;
    var listItems = ['Bakery', 'CrossFit', 'Salon', 'Dental', 'Brewery'];
    for (var li = 0; li < listItems.length; li++) {
      var ly = phoneY + 16*s + li*12*s;
      ctx.fillStyle = aClr(p,0.035);
      ctx.fillRect(screenX, ly, screenW, 10*s);
      ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s;
      ctx.strokeRect(screenX, ly, screenW, 10*s);
      // App icon dot
      ctx.fillStyle = aClr(p,0.25);
      ctx.beginPath(); ctx.arc(screenX+4*s, ly+5*s, 2*s, 0, Math.PI*2); ctx.fill();
      // Name
      ctx.fillStyle = tClr(p,0.35); ctx.font = '300 '+(3.5*s)+'px Inter, sans-serif';
      ctx.fillText(listItems[li], screenX+9*s, ly+6.5*s);
    }

    // Side floating cards (left & right of phone)
    var verticals = [
      {label: 'FOOD', x: w*0.06, y: h*0.32, color: '220,180,100'},
      {label: 'FITNESS', x: w*0.06, y: h*0.52, color: '100,200,150'},
      {label: 'BEAUTY', x: w*0.78, y: h*0.32, color: '200,140,180'},
      {label: 'HEALTH', x: w*0.78, y: h*0.52, color: '140,180,220'},
      {label: 'DRINKS', x: w*0.06, y: h*0.72, color: '180,160,120'},
      {label: 'RETAIL', x: w*0.78, y: h*0.72, color: '160,200,180'}
    ];
    for (var vi = 0; vi < verticals.length; vi++) {
      var v = verticals[vi];
      var vw = 28*s, vh = 14*s;
      ctx.fillStyle = 'rgba('+v.color+',0.025)'; ctx.fillRect(v.x, v.y, vw, vh);
      ctx.strokeStyle = 'rgba('+v.color+',0.08)'; ctx.lineWidth = 0.5*s; ctx.strokeRect(v.x, v.y, vw, vh);
      ctx.fillStyle = 'rgba('+v.color+',0.35)'; ctx.font = '200 '+(3.5*s)+'px Inter, sans-serif';
      ctx.textAlign = 'center'; ctx.fillText(v.label, v.x+vw/2, v.y+vh/2+1.5*s); ctx.textAlign = 'left';
    }

    // Bottom bar - stats
    var barY = h - 22*s;
    ctx.fillStyle = aClr(p,0.03); ctx.fillRect(0, barY, w, 22*s);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, barY); ctx.lineTo(w, barY); ctx.stroke();
    ctx.fillStyle = aClr(p,0.35); ctx.font = (5*s)+'px monospace';
    ctx.fillText('6 verticals', 10*s, barY+14*s);
    ctx.textAlign = 'right'; ctx.fillText('$199/mo', w-10*s, barY+14*s); ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p,0.15));
  }

  // ---- MARKET DASHBOARD: Trading terminal ----
  function drawMarketDash(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(0, 220, 160);

    // Terminal bg
    var bg = ctx.createLinearGradient(0, 0, 0, h);
    if (p.light) { bg.addColorStop(0, '#e6eae8'); bg.addColorStop(0.5, '#e2e8e6'); bg.addColorStop(1, '#e0e6e4'); }
    else { bg.addColorStop(0, '#080a0c'); bg.addColorStop(0.5, '#060a0e'); bg.addColorStop(1, '#04080c'); }
    ctx.fillStyle = bg; ctx.fillRect(0, 0, w, h);

    // Scanline overlay
    ctx.fillStyle = p.light ? 'rgba(0,180,120,0.008)' : 'rgba(0,255,180,0.003)';
    for (var sl = 0; sl < h; sl += 2*s) { ctx.fillRect(0, sl, w, 1*s); }

    // Header bar
    var hGrad = ctx.createLinearGradient(0, 0, w, 0);
    hGrad.addColorStop(0, aClr(p,0.06)); hGrad.addColorStop(1, aClr(p,0.01));
    ctx.fillStyle = hGrad; ctx.fillRect(0, 0, w, 24*s);
    ctx.strokeStyle = aClr(p,0.08); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 24*s); ctx.lineTo(w, 24*s); ctx.stroke();
    ctx.fillStyle = aClr(p,0.5); ctx.fillRect(0, 0, 3*s, 24*s);
    ctx.fillStyle = tClr(p,0.65); ctx.font = '500 '+(8*s)+'px Inter, sans-serif';
    ctx.fillText('Market Terminal', 14*s, 16*s);
    // Live dot
    ctx.beginPath(); ctx.arc(w-12*s, 12*s, 2.5*s, 0, Math.PI*2);
    ctx.fillStyle = p.light ? '#00a070' : '#00dc96'; ctx.fill();
    ctx.fillStyle = (p.light ? 'rgba(0,160,110,0.5)' : 'rgba(0,220,150,0.5)'); ctx.font = (5.5*s)+'px monospace';
    ctx.textAlign = 'right'; ctx.fillText('LIVE', w-18*s, 15*s); ctx.textAlign = 'left';

    // Candlestick chart (main area)
    var chartX = 8*s, chartY = 32*s, chartW = w*0.62, chartH = h*0.52;
    ctx.fillStyle = aClr(p,0.01); ctx.fillRect(chartX, chartY, chartW, chartH);
    ctx.strokeStyle = aClr(p,0.04); ctx.lineWidth = 0.5*s; ctx.strokeRect(chartX, chartY, chartW, chartH);

    // Horizontal grid lines
    for (var gl = 0; gl < 5; gl++) {
      var gly = chartY + gl*(chartH/5);
      ctx.strokeStyle = aClr(p,0.025); ctx.lineWidth = 0.5*s;
      ctx.beginPath(); ctx.moveTo(chartX, gly); ctx.lineTo(chartX+chartW, gly); ctx.stroke();
    }

    // Candlesticks
    var candleW = 4*s, gap = 1.5*s;
    var nCandles = Math.floor(chartW / (candleW + gap));
    var basePrice = chartH*0.5;
    for (var ci = 0; ci < nCandles; ci++) {
      var cx = chartX + 4*s + ci*(candleW+gap);
      var noise = Math.sin(ci*0.8+1.2)*0.35 + Math.sin(ci*0.3+0.5)*0.2;
      var open = basePrice + noise*chartH*0.3;
      var close = open + (Math.sin(ci*1.7+2.3)*0.08)*chartH;
      var high = Math.min(open, close) - Math.abs(Math.sin(ci*2.1))*10*s;
      var low = Math.max(open, close) + Math.abs(Math.cos(ci*1.9))*10*s;
      var bullish = close < open; // inverted because Y axis is flipped
      ctx.strokeStyle = bullish ? (p.light ? 'rgba(0,160,110,0.3)' : 'rgba(0,220,150,0.3)') : 'rgba(248,113,113,0.3)';
      ctx.lineWidth = 0.5*s;
      // Wick
      ctx.beginPath(); ctx.moveTo(cx+candleW/2, chartY+high); ctx.lineTo(cx+candleW/2, chartY+low); ctx.stroke();
      // Body
      ctx.fillStyle = bullish ? (p.light ? 'rgba(0,160,110,0.15)' : 'rgba(0,220,150,0.15)') : 'rgba(248,113,113,0.12)';
      var bodyTop = chartY + Math.min(open, close), bodyH = Math.abs(close - open) || 1*s;
      ctx.fillRect(cx, bodyTop, candleW, bodyH);
      ctx.strokeRect(cx, bodyTop, candleW, bodyH);
    }

    // Order book (right side)
    var obX = w*0.66, obY = 32*s, obW = w*0.32, obH = chartH;
    ctx.fillStyle = aClr(p,0.01); ctx.fillRect(obX, obY, obW, obH);
    ctx.strokeStyle = aClr(p,0.04); ctx.lineWidth = 0.5*s; ctx.strokeRect(obX, obY, obW, obH);
    ctx.fillStyle = aClr(p,0.3); ctx.font = '200 '+(4.5*s)+'px Inter, sans-serif';
    ctx.fillText('ORDER BOOK', obX+6*s, obY+10*s);

    // Asks (red) top half
    for (var ai = 0; ai < 6; ai++) {
      var ay = obY + 16*s + ai*7*s;
      var aBarW = (0.3 + Math.sin(ai*1.7+0.5)*0.15)*obW*0.65;
      ctx.fillStyle = 'rgba(248,113,113,0.06)'; ctx.fillRect(obX+obW-aBarW-4*s, ay, aBarW, 5*s);
      ctx.fillStyle = 'rgba(248,113,113,0.3)'; ctx.font = (3.5*s)+'px monospace';
      ctx.textAlign = 'right'; ctx.fillText((67200-ai*12).toFixed(0), obX+obW-6*s, ay+4*s);
      ctx.textAlign = 'left';
    }
    // Spread line
    var spreadY = obY + 16*s + 6*7*s + 2*s;
    ctx.strokeStyle = 'rgba(201,168,76,0.15)'; ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(obX+4*s, spreadY); ctx.lineTo(obX+obW-4*s, spreadY); ctx.stroke();
    ctx.fillStyle = 'rgba(201,168,76,0.3)'; ctx.font = (3*s)+'px monospace';
    ctx.textAlign = 'center'; ctx.fillText('SPREAD', obX+obW/2, spreadY-2*s); ctx.textAlign = 'left';
    // Bids (green) bottom half
    for (var bi = 0; bi < 6; bi++) {
      var by = spreadY + 6*s + bi*7*s;
      var bBarW = (0.3 + Math.sin(bi*2.1+1.2)*0.15)*obW*0.65;
      ctx.fillStyle = (p.light ? 'rgba(0,160,110,0.06)' : 'rgba(0,220,150,0.06)'); ctx.fillRect(obX+4*s, by, bBarW, 5*s);
      ctx.fillStyle = (p.light ? 'rgba(0,160,110,0.3)' : 'rgba(0,220,150,0.3)'); ctx.font = (3.5*s)+'px monospace';
      ctx.fillText((67140-bi*12).toFixed(0), obX+6*s, by+4*s);
    }

    // Trade tape (bottom)
    var tapeY = chartY + chartH + 8*s;
    ctx.fillStyle = aClr(p,0.025); ctx.fillRect(8*s, tapeY, w-16*s, h-tapeY-28*s);
    ctx.strokeStyle = aClr(p,0.04); ctx.lineWidth = 0.5*s;
    ctx.strokeRect(8*s, tapeY, w-16*s, h-tapeY-28*s);
    ctx.fillStyle = aClr(p,0.3); ctx.font = '200 '+(4.5*s)+'px Inter, sans-serif';
    ctx.fillText('TRADE TAPE', 14*s, tapeY+10*s);
    // Tape entries
    var trades = ['BUY 0.15 @ 67,148', 'SELL 0.42 @ 67,156', 'BUY 1.20 @ 67,144', 'SELL 0.08 @ 67,160'];
    for (var ti = 0; ti < trades.length; ti++) {
      var ty = tapeY + 16*s + ti*6.5*s;
      var isBuy = trades[ti].indexOf('BUY') === 0;
      ctx.fillStyle = isBuy ? (p.light ? 'rgba(0,160,110,0.25)' : 'rgba(0,220,150,0.25)') : 'rgba(248,113,113,0.25)';
      ctx.font = (3.5*s)+'px monospace';
      ctx.fillText(trades[ti], 14*s, ty);
    }

    // Bottom stat bar
    var barY = h - 22*s;
    ctx.fillStyle = aClr(p,0.03); ctx.fillRect(0, barY, w, 22*s);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, barY); ctx.lineTo(w, barY); ctx.stroke();
    ctx.fillStyle = (p.light ? 'rgba(0,160,110,0.35)' : 'rgba(0,220,150,0.35)'); ctx.font = (5*s)+'px monospace';
    ctx.fillText('0 deps', 10*s, barY+14*s);
    ctx.textAlign = 'center'; ctx.fillText('60 FPS', w/2, barY+14*s);
    ctx.textAlign = 'right'; ctx.fillText('WebSocket', w-10*s, barY+14*s); ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p,0.12));
  }

  // ---- AUTON: Autonomous background worker ----
  function drawAuton(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(160, 120, 220);

    // Purple-tinted bg (AI aesthetic)
    var bg = ctx.createLinearGradient(0, 0, w*0.4, h);
    if (p.light) { bg.addColorStop(0, '#e8e4ee'); bg.addColorStop(0.5, '#e4e0ec'); bg.addColorStop(1, '#e6e2ea'); }
    else { bg.addColorStop(0, '#090710'); bg.addColorStop(0.5, '#0b0912'); bg.addColorStop(1, '#07060c'); }
    ctx.fillStyle = bg; ctx.fillRect(0, 0, w, h);

    // Neural network dots & connections (background pattern)
    var nodes = [];
    for (var ni = 0; ni < 30; ni++) {
      nodes.push({
        x: (Math.sin(ni*4.7+1.3)*0.4+0.5)*w,
        y: (Math.sin(ni*3.2+0.8)*0.4+0.5)*h
      });
    }
    // Connections
    ctx.strokeStyle = aClr(p,0.015); ctx.lineWidth = 0.5*s;
    for (var ci = 0; ci < nodes.length; ci++) {
      for (var cj = ci+1; cj < nodes.length; cj++) {
        var dx = nodes[ci].x - nodes[cj].x, dy = nodes[ci].y - nodes[cj].y;
        if (dx*dx+dy*dy < (80*s)*(80*s)) {
          ctx.beginPath(); ctx.moveTo(nodes[ci].x, nodes[ci].y); ctx.lineTo(nodes[cj].x, nodes[cj].y); ctx.stroke();
        }
      }
    }
    // Nodes
    for (var nj = 0; nj < nodes.length; nj++) {
      ctx.fillStyle = aClr(p,0.04);
      ctx.beginPath(); ctx.arc(nodes[nj].x, nodes[nj].y, 1.5*s, 0, Math.PI*2); ctx.fill();
    }

    // Header bar
    var hGrad = ctx.createLinearGradient(0, 0, w, 0);
    hGrad.addColorStop(0, aClr(p,0.06)); hGrad.addColorStop(1, aClr(p,0.01));
    ctx.fillStyle = hGrad; ctx.fillRect(0, 0, w, 24*s);
    ctx.strokeStyle = aClr(p,0.08); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 24*s); ctx.lineTo(w, 24*s); ctx.stroke();
    ctx.fillStyle = aClr(p,0.5); ctx.fillRect(0, 0, 3*s, 24*s);
    ctx.fillStyle = tClr(p,0.65); ctx.font = '500 '+(8*s)+'px Inter, sans-serif';
    ctx.fillText('Auton', 14*s, 16*s);
    ctx.fillStyle = aClr(p,0.35); ctx.font = '200 '+(6*s)+'px Inter, sans-serif';
    ctx.textAlign = 'right'; ctx.fillText('AUTONOMOUS', w-14*s, 16*s); ctx.textAlign = 'left';

    // Agent pipeline (central feature)
    var agents = [
      {name: 'Scanner', icon: '\u25CE', color: '120,180,220'},
      {name: 'Planner', icon: '\u25C7', color: '160,120,220'},
      {name: 'Executor', icon: '\u25B7', color: '200,120,180'},
      {name: 'Tester', icon: '\u25A1', color: '120,200,160'},
      {name: 'Reviewer', icon: '\u25CB', color: '220,180,100'},
      {name: 'Monitor', icon: '\u25C9', color: '100,180,200'}
    ];
    var pipeY = h*0.32;
    var pipeH = 14*s;
    var agentW = (w - 20*s) / agents.length;
    for (var ai = 0; ai < agents.length; ai++) {
      var ag = agents[ai];
      var ax = 10*s + ai*agentW;
      // Agent box
      ctx.fillStyle = 'rgba('+ag.color+',0.025)';
      ctx.fillRect(ax+1*s, pipeY, agentW-2*s, pipeH);
      ctx.strokeStyle = 'rgba('+ag.color+',0.1)'; ctx.lineWidth = 0.5*s;
      ctx.strokeRect(ax+1*s, pipeY, agentW-2*s, pipeH);
      // Icon
      ctx.fillStyle = 'rgba('+ag.color+',0.35)'; ctx.font = (6*s)+'px monospace';
      ctx.textAlign = 'center'; ctx.fillText(ag.icon, ax+agentW/2, pipeY+6*s);
      // Name
      ctx.fillStyle = 'rgba('+ag.color+',0.4)'; ctx.font = '200 '+(3*s)+'px Inter, sans-serif';
      ctx.fillText(ag.name, ax+agentW/2, pipeY+12*s);
      // Arrow between agents
      if (ai < agents.length-1) {
        ctx.strokeStyle = aClr(p,0.08); ctx.lineWidth = 0.5*s;
        ctx.beginPath(); ctx.moveTo(ax+agentW-1*s, pipeY+pipeH/2);
        ctx.lineTo(ax+agentW+1*s, pipeY+pipeH/2); ctx.stroke();
      }
    }
    ctx.textAlign = 'left';

    // Safety model visualization
    var safeY = pipeY + pipeH + 18*s;
    ctx.fillStyle = aClr(p,0.02); ctx.fillRect(10*s, safeY, w-20*s, 28*s);
    ctx.strokeStyle = aClr(p,0.05); ctx.lineWidth = 0.5*s;
    ctx.strokeRect(10*s, safeY, w-20*s, 28*s);
    ctx.fillStyle = aClr(p,0.25); ctx.font = '200 '+(4.5*s)+'px Inter, sans-serif';
    ctx.fillText('SAFETY LAYERS', 16*s, safeY+10*s);

    var layers = ['DRY_RUN', 'SafetyGuard', 'Kill Switch', 'Cred Isolation', 'Audit', 'Rollback'];
    var layerW = (w - 30*s) / layers.length;
    for (var li = 0; li < layers.length; li++) {
      var lx = 15*s + li*layerW;
      ctx.fillStyle = 'rgba(0,220,150,0.04)'; ctx.fillRect(lx, safeY+14*s, layerW-3*s, 10*s);
      ctx.strokeStyle = 'rgba(0,220,150,0.08)'; ctx.lineWidth = 0.5*s; ctx.strokeRect(lx, safeY+14*s, layerW-3*s, 10*s);
      ctx.fillStyle = 'rgba(0,220,150,0.35)'; ctx.font = (2.8*s)+'px monospace';
      ctx.textAlign = 'center'; ctx.fillText(layers[li], lx+(layerW-3*s)/2, safeY+21*s);
    }
    ctx.textAlign = 'left';

    // Activity log at bottom
    var logY = safeY + 34*s;
    var logEntries = [
      {t:'14:22:01', msg:'Scanner → found 3 improvements in profit-desk/', color:'120,180,220'},
      {t:'14:22:08', msg:'Planner → drafted PR #47: add retry logic', color:'160,120,220'},
      {t:'14:22:15', msg:'Executor → applied patch (DRY_RUN)', color:'200,120,180'},
      {t:'14:22:19', msg:'Tester → all 42 tests passing', color:'120,200,160'}
    ];
    for (var ei = 0; ei < logEntries.length; ei++) {
      var entry = logEntries[ei];
      var ey = logY + ei*7*s;
      ctx.fillStyle = 'rgba('+entry.color+',0.06)'; ctx.font = (3*s)+'px monospace';
      ctx.fillText(entry.t, 10*s, ey);
      ctx.fillStyle = 'rgba('+entry.color+',0.25)'; ctx.font = (3*s)+'px monospace';
      ctx.fillText(entry.msg, 36*s, ey);
    }

    // Bottom stat bar
    var barY = h - 22*s;
    ctx.fillStyle = aClr(p,0.03); ctx.fillRect(0, barY, w, 22*s);
    ctx.strokeStyle = aClr(p,0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, barY); ctx.lineTo(w, barY); ctx.stroke();
    ctx.fillStyle = aClr(p,0.35); ctx.font = (5*s)+'px monospace';
    ctx.fillText('6 agents', 10*s, barY+14*s);
    ctx.textAlign = 'center'; ctx.fillText('14B LLM', w/2, barY+14*s);
    ctx.textAlign = 'right'; ctx.fillText('RTX 4070', w-10*s, barY+14*s); ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p,0.12));
  }

  // ---- OPERATOR CONSOLE: Service health command center ----
  function drawOperatorConsole(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(80, 200, 140);

    // Dark terminal bg
    var bg = ctx.createLinearGradient(0, 0, 0, h);
    if (p.light) { bg.addColorStop(0, '#e2ece7'); bg.addColorStop(1, '#dde8e3'); }
    else { bg.addColorStop(0, '#060d0a'); bg.addColorStop(1, '#040908'); }
    ctx.fillStyle = bg; ctx.fillRect(0, 0, w, h);

    // Subtle grid
    ctx.strokeStyle = aClr(p, 0.025); ctx.lineWidth = 0.5*s;
    for (var gx = 0; gx < w; gx += 20*s) { ctx.beginPath(); ctx.moveTo(gx, 0); ctx.lineTo(gx, h); ctx.stroke(); }
    for (var gy = 0; gy < h; gy += 20*s) { ctx.beginPath(); ctx.moveTo(0, gy); ctx.lineTo(w, gy); ctx.stroke(); }

    // Header bar
    var hGrad = ctx.createLinearGradient(0, 0, w, 0);
    hGrad.addColorStop(0, aClr(p, 0.08)); hGrad.addColorStop(1, aClr(p, 0.02));
    ctx.fillStyle = hGrad; ctx.fillRect(0, 0, w, 24*s);
    ctx.strokeStyle = aClr(p, 0.1); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 24*s); ctx.lineTo(w, 24*s); ctx.stroke();
    ctx.fillStyle = aClr(p, 0.55); ctx.fillRect(0, 0, 3*s, 24*s);
    ctx.fillStyle = tClr(p, 0.65); ctx.font = '500 '+(8*s)+'px Inter, sans-serif';
    ctx.fillText('Operator Console', 14*s, 16*s);
    ctx.fillStyle = aClr(p, 0.4); ctx.font = '200 '+(6*s)+'px Inter, sans-serif';
    ctx.textAlign = 'right'; ctx.fillText('CONTROL PLANE', w-14*s, 16*s); ctx.textAlign = 'left';

    // Tab strip (5-tab console)
    var ocTabsY = 24*s, ocTabsH = 14*s;
    ctx.fillStyle = aClr(p, 0.025); ctx.fillRect(0, ocTabsY, w, ocTabsH);
    ctx.strokeStyle = aClr(p, 0.07); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, ocTabsY+ocTabsH); ctx.lineTo(w, ocTabsY+ocTabsH); ctx.stroke();
    var ocTabs = ['Health','Approvals','Logs','Audit','Config'];
    var ocTabW = w / ocTabs.length;
    for (var oct = 0; oct < ocTabs.length; oct++) {
      var octx = oct*ocTabW;
      if (oct === 0) {
        ctx.fillStyle = aClr(p, 0.08); ctx.fillRect(octx, ocTabsY, ocTabW, ocTabsH);
        ctx.fillStyle = aClr(p, 0.7); ctx.fillRect(octx, ocTabsY+ocTabsH-1.5*s, ocTabW, 1.5*s);
      }
      ctx.fillStyle = oct === 0 ? tClr(p, 0.6) : tClr(p, 0.25);
      ctx.font = (oct === 0 ? '500 ' : '300 ')+(5.5*s)+'px Inter, sans-serif';
      ctx.textAlign = 'center'; ctx.fillText(ocTabs[oct], octx+ocTabW/2, ocTabsY+9.5*s);
      // Pending count badge on Approvals tab
      if (oct === 1) {
        ctx.beginPath(); ctx.arc(octx+ocTabW/2+18*s, ocTabsY+5*s, 3.5*s, 0, Math.PI*2);
        ctx.fillStyle = 'rgba(220,180,60,0.7)'; ctx.fill();
        ctx.fillStyle = (p.light ? 'rgba(255,255,255,0.95)' : 'rgba(13,13,14,0.95)');
        ctx.font = '600 '+(3.5*s)+'px Inter, sans-serif';
        ctx.fillText('3', octx+ocTabW/2+18*s, ocTabsY+6.5*s);
      }
    }
    ctx.textAlign = 'left';

    // Service health grid (4 columns x 3 rows)
    var services = [
      {name:'Sovereign Hub', st:'online', up:'99.9%', c:'0,200,120'},
      {name:'Open-WebUI', st:'online', up:'99.7%', c:'0,200,120'},
      {name:'Session Atlas', st:'online', up:'98.4%', c:'0,200,120'},
      {name:'Home Hub', st:'online', up:'97.1%', c:'0,200,120'},
      {name:'Auton', st:'online', up:'99.2%', c:'0,200,120'},
      {name:'Ollama', st:'online', up:'100%', c:'0,200,120'},
      {name:'Git Sync', st:'online', up:'99.8%', c:'0,200,120'},
      {name:'profit-desk', st:'standby', up:'DRY', c:'220,180,60'},
      {name:'master-trade', st:'standby', up:'DRY', c:'220,180,60'},
      {name:'Sovereign DB', st:'online', up:'99.5%', c:'0,200,120'},
      {name:'MoltBot', st:'online', up:'98.9%', c:'0,200,120'},
      {name:'Enclave', st:'offline', up:'---', c:'180,60,60'}
    ];
    var cols = 4, rows = 3;
    var gridX = 10*s, gridY = 44*s;
    var cellW = (w - 20*s) / cols, cellH = (h - gridY - 30*s) / rows;
    for (var si = 0; si < services.length; si++) {
      var svc = services[si];
      var col = si % cols, row = Math.floor(si / cols);
      var cx = gridX + col*cellW, cy = gridY + row*cellH;
      // Cell bg
      ctx.fillStyle = 'rgba('+svc.c+',0.03)'; ctx.fillRect(cx+1*s, cy+1*s, cellW-2*s, cellH-2*s);
      ctx.strokeStyle = 'rgba('+svc.c+',0.1)'; ctx.lineWidth = 0.5*s;
      ctx.strokeRect(cx+1*s, cy+1*s, cellW-2*s, cellH-2*s);
      // Status dot
      ctx.fillStyle = 'rgba('+svc.c+',0.55)';
      ctx.beginPath(); ctx.arc(cx+7*s, cy+7*s, 2.5*s, 0, Math.PI*2); ctx.fill();
      // Name
      ctx.fillStyle = 'rgba('+svc.c+',0.45)'; ctx.font = '400 '+(4*s)+'px Inter, sans-serif';
      ctx.fillText(svc.name, cx+13*s, cy+9*s);
      // Uptime
      ctx.fillStyle = 'rgba('+svc.c+',0.25)'; ctx.font = (3.5*s)+'px monospace';
      ctx.fillText(svc.up, cx+7*s, cy+cellH-6*s);
    }

    // Bottom bar
    var barY = h - 22*s;
    ctx.fillStyle = aClr(p, 0.04); ctx.fillRect(0, barY, w, 22*s);
    ctx.strokeStyle = aClr(p, 0.08); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, barY); ctx.lineTo(w, barY); ctx.stroke();
    ctx.fillStyle = 'rgba(0,200,120,0.4)'; ctx.font = (5*s)+'px monospace';
    ctx.fillText('11/12 online', 10*s, barY+14*s);
    ctx.textAlign = 'center'; ctx.fillStyle = aClr(p, 0.3); ctx.fillText('24/7 monitoring', w/2, barY+14*s);
    ctx.textAlign = 'right'; ctx.fillText('v2.4.1', w-10*s, barY+14*s); ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p, 0.15));
  }

  // ---- SESSION ATLAS: Session activity heatmap ----
  function drawSessionAtlas(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(201, 168, 76);

    // Warm bg
    var bg = ctx.createLinearGradient(0, 0, 0, h);
    if (p.light) { bg.addColorStop(0, '#edeade'); bg.addColorStop(1, '#e8e5d9'); }
    else { bg.addColorStop(0, '#0d0b06'); bg.addColorStop(1, '#090807'); }
    ctx.fillStyle = bg; ctx.fillRect(0, 0, w, h);

    // Header bar
    var hGrad = ctx.createLinearGradient(0, 0, w, 0);
    hGrad.addColorStop(0, aClr(p, 0.07)); hGrad.addColorStop(1, aClr(p, 0.01));
    ctx.fillStyle = hGrad; ctx.fillRect(0, 0, w, 24*s);
    ctx.strokeStyle = aClr(p, 0.09); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 24*s); ctx.lineTo(w, 24*s); ctx.stroke();
    ctx.fillStyle = aClr(p, 0.5); ctx.fillRect(0, 0, 3*s, 24*s);
    ctx.fillStyle = tClr(p, 0.65); ctx.font = '500 '+(8*s)+'px Inter, sans-serif';
    ctx.fillText('Session Atlas', 14*s, 16*s);
    ctx.fillStyle = aClr(p, 0.35); ctx.font = '200 '+(6*s)+'px Inter, sans-serif';
    ctx.textAlign = 'right'; ctx.fillText('INTELLIGENCE', w-14*s, 16*s); ctx.textAlign = 'left';

    // Activity heatmap (14 weeks x 7 days)
    var mapX = 10*s, mapY = 30*s;
    var weeks = 14, days = 7;
    var cellSz = Math.min((w - 20*s) / weeks, 10*s);
    var seed = [8,5,12,3,9,15,4,11,7,14,6,13,10,2,8,5,12,3,9,15,4,11,7,14,6,0,0,0,0,3,
                7,12,4,9,3,14,1,8,12,5,11,7,15,4,2,9,6,13,0,0,0,8,5,12,3,9,15,4,11,7,
                0,0,0,0,0,0,0,5,3,10,8,15,4,12,6,11,2,9,7,14,1,8,5,12,3,9,15,4,11,7,
                9,6,14,3,10,2,13,5,11,7,0,0,0,4,8,15,2,9,6,13,1,10,5,12,3,14,7,11,4,8];
    for (var wk = 0; wk < weeks; wk++) {
      for (var dy = 0; dy < days; dy++) {
        var intensity = seed[(wk*days+dy) % seed.length] / 15;
        var bx = mapX + wk*(cellSz+1*s), by = mapY + dy*(cellSz+1*s);
        ctx.fillStyle = intensity < 0.01
          ? aClr(p, 0.04)
          : 'rgba(201,168,76,'+(0.08 + intensity*0.45)+')';
        ctx.fillRect(bx, by, cellSz, cellSz);
      }
    }

    // Day labels
    var dayLabels = ['M','T','W','T','F','S','S'];
    ctx.fillStyle = aClr(p, 0.3); ctx.font = (3*s)+'px monospace';
    for (var dl = 0; dl < 7; dl++) {
      ctx.fillText(dayLabels[dl], mapX - 6*s, mapY + dl*(cellSz+1*s) + cellSz*0.7);
    }

    // Project breakdown bars below heatmap
    var barSectionY = mapY + days*(cellSz+1*s) + 10*s;
    var projects = [
      {name:'FisherSovereign', pct:0.28, c:'201,168,76'},
      {name:'Bloodlines', pct:0.18, c:'160,120,220'},
      {name:'LocalClaude', pct:0.15, c:'80,160,220'},
      {name:'Trading', pct:0.12, c:'80,200,140'},
      {name:'HomeHub', pct:0.10, c:'100,180,220'},
      {name:'Others', pct:0.17, c:'140,140,140'}
    ];
    var barTotalW = w - 20*s;
    var bX = 10*s;
    for (var pi = 0; pi < projects.length; pi++) {
      var proj = projects[pi];
      var bW = barTotalW * proj.pct;
      ctx.fillStyle = 'rgba('+proj.c+',0.35)';
      ctx.fillRect(bX, barSectionY, bW - 1*s, 8*s);
      bX += bW;
    }
    // Bar label
    ctx.fillStyle = aClr(p, 0.3); ctx.font = '200 '+(3.5*s)+'px Inter, sans-serif';
    ctx.fillText('Session distribution across 20 active projects', 10*s, barSectionY + 14*s);

    // Sparkline: sessions per week
    var spkY = barSectionY + 20*s;
    var spkH = h - spkY - 26*s;
    var spkData = [4,6,5,9,7,12,8,11,10,15,12,14,9,13];
    var spkMax = 15;
    ctx.strokeStyle = aClr(p, 0.08); ctx.lineWidth = 0.5*s;
    ctx.strokeRect(10*s, spkY, w-20*s, spkH);
    ctx.fillStyle = aClr(p, 0.2); ctx.font = '200 '+(4*s)+'px Inter, sans-serif';
    ctx.fillText('WEEKLY SESSIONS', 16*s, spkY+8*s);
    var spkW = (w - 30*s) / spkData.length;
    ctx.beginPath();
    for (var sk = 0; sk < spkData.length; sk++) {
      var sx2 = 15*s + sk*spkW + spkW/2;
      var sy2 = spkY + spkH - 4*s - (spkData[sk]/spkMax)*(spkH-14*s);
      if (sk === 0) ctx.moveTo(sx2, sy2); else ctx.lineTo(sx2, sy2);
    }
    ctx.strokeStyle = aClr(p, 0.4); ctx.lineWidth = 1*s; ctx.stroke();

    // Bottom bar
    var barY2 = h - 22*s;
    ctx.fillStyle = aClr(p, 0.03); ctx.fillRect(0, barY2, w, 22*s);
    ctx.strokeStyle = aClr(p, 0.07); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, barY2); ctx.lineTo(w, barY2); ctx.stroke();
    ctx.fillStyle = aClr(p, 0.35); ctx.font = (5*s)+'px monospace';
    ctx.fillText('81 sessions', 10*s, barY2+14*s);
    ctx.textAlign = 'center'; ctx.fillText('20 projects', w/2, barY2+14*s);
    ctx.textAlign = 'right'; ctx.fillText(':8092', w-10*s, barY2+14*s); ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p, 0.15));
  }

  // ---- PRIVATE INTELLIGENCE PLATFORM: Local LLM chat UI ----
  function drawPrivateIntelligence(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(80, 160, 220);

    // Deep blue bg
    var bg = ctx.createLinearGradient(0, 0, w*0.3, h);
    if (p.light) { bg.addColorStop(0, '#e2e8f0'); bg.addColorStop(1, '#dce4ec'); }
    else { bg.addColorStop(0, '#060a10'); bg.addColorStop(1, '#04070d'); }
    ctx.fillStyle = bg; ctx.fillRect(0, 0, w, h);

    // Subtle scan-line pattern
    for (var sl = 0; sl < h; sl += 4*s) {
      ctx.fillStyle = p.light ? 'rgba(80,160,220,0.015)' : 'rgba(80,160,220,0.008)';
      ctx.fillRect(0, sl, w, 1*s);
    }

    // Header bar
    var hGrad = ctx.createLinearGradient(0, 0, w, 0);
    hGrad.addColorStop(0, aClr(p, 0.07)); hGrad.addColorStop(1, aClr(p, 0.01));
    ctx.fillStyle = hGrad; ctx.fillRect(0, 0, w, 24*s);
    ctx.strokeStyle = aClr(p, 0.09); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 24*s); ctx.lineTo(w, 24*s); ctx.stroke();
    ctx.fillStyle = aClr(p, 0.5); ctx.fillRect(0, 0, 3*s, 24*s);
    ctx.fillStyle = tClr(p, 0.65); ctx.font = '500 '+(8*s)+'px Inter, sans-serif';
    ctx.fillText('Private Intelligence', 14*s, 16*s);
    ctx.fillStyle = 'rgba(0,220,100,0.45)'; ctx.font = '200 '+(5*s)+'px monospace';
    ctx.textAlign = 'right'; ctx.fillText('100% LOCAL', w-14*s, 16*s); ctx.textAlign = 'left';

    // Sidebar: model list
    var sideW = 56*s;
    ctx.fillStyle = aClr(p, 0.03); ctx.fillRect(0, 24*s, sideW, h-24*s);
    ctx.strokeStyle = aClr(p, 0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(sideW, 24*s); ctx.lineTo(sideW, h); ctx.stroke();
    ctx.fillStyle = aClr(p, 0.25); ctx.font = '200 '+(3.5*s)+'px Inter, sans-serif';
    ctx.fillText('MODELS', 6*s, 34*s);
    var models = [
      {name:'qwen2.5:72b', provider:'Ollama', active:true},
      {name:'llama3.3:70b', provider:'Ollama', active:false},
      {name:'deepseek-r1', provider:'Ollama', active:false},
      {name:'gemma3:27b', provider:'Ollama', active:false},
      {name:'phi4:14b', provider:'Ollama', active:false}
    ];
    for (var mi = 0; mi < models.length; mi++) {
      var m = models[mi];
      var my = 40*s + mi*18*s;
      if (m.active) { ctx.fillStyle = aClr(p, 0.07); ctx.fillRect(1*s, my-2*s, sideW-2*s, 16*s); }
      ctx.fillStyle = m.active ? aClr(p, 0.6) : aClr(p, 0.25);
      ctx.font = '400 '+(3.5*s)+'px monospace';
      ctx.fillText(m.name, 6*s, my+7*s);
      ctx.fillStyle = m.active ? 'rgba(0,220,100,0.4)' : aClr(p, 0.15);
      ctx.font = (3*s)+'px Inter, sans-serif';
      ctx.fillText(m.provider, 6*s, my+12*s);
    }

    // Chat area
    var chatX = sideW + 6*s;
    var chatW = w - chatX - 6*s;
    var msgs = [
      {role:'user', text:'Analyze trading patterns from the last 30 days', y:30*s},
      {role:'ai', text:'Reviewing profit-desk logs... Found 3 high-confidence signals on BTC/ETH correlation. Recommendation: increase position size on next SOL breakout above $185.', y:46*s},
      {role:'user', text:'What is the risk exposure on current open positions?', y:80*s},
      {role:'ai', text:'Total exposure: $12,400. DRY_RUN mode active — no live capital at risk. All signals are paper-traded.', y:96*s}
    ];
    for (var ci2 = 0; ci2 < msgs.length; ci2++) {
      var msg = msgs[ci2];
      var isUser = msg.role === 'user';
      var msgX = chatX + (isUser ? chatW*0.12 : 2*s);
      var msgW2 = chatW * 0.86;
      // Bubble bg
      ctx.fillStyle = isUser ? aClr(p, 0.06) : aClr(p, 0.03);
      ctx.fillRect(msgX, 24*s + msg.y, msgW2, 14*s);
      ctx.strokeStyle = aClr(p, isUser ? 0.12 : 0.05); ctx.lineWidth = 0.5*s;
      ctx.strokeRect(msgX, 24*s + msg.y, msgW2, 14*s);
      // Role label
      ctx.fillStyle = isUser ? aClr(p, 0.4) : 'rgba(0,200,120,0.35)';
      ctx.font = '600 '+(3.5*s)+'px Inter, sans-serif';
      ctx.fillText(isUser ? 'You' : 'Qwen', msgX+4*s, 24*s + msg.y + 6*s);
      // Message text (truncated)
      ctx.fillStyle = tClr(p, 0.45); ctx.font = (3.2*s)+'px Inter, sans-serif';
      ctx.fillText(msg.text.substring(0, 52), msgX+4*s, 24*s + msg.y + 11*s);
    }

    // Input bar
    var inputY = h - 44*s;
    ctx.fillStyle = aClr(p, 0.04); ctx.fillRect(chatX, inputY, chatW, 16*s);
    ctx.strokeStyle = aClr(p, 0.1); ctx.lineWidth = 0.5*s;
    ctx.strokeRect(chatX, inputY, chatW, 16*s);
    ctx.fillStyle = aClr(p, 0.2); ctx.font = (3.5*s)+'px Inter, sans-serif';
    ctx.fillText('Message Claude...', chatX+6*s, inputY+10*s);
    // Cursor blink
    ctx.fillStyle = aClr(p, 0.5); ctx.fillRect(chatX+6*s+38*s, inputY+4*s, 1*s, 7*s);

    // Bottom bar
    var barY3 = h - 22*s;
    ctx.fillStyle = aClr(p, 0.04); ctx.fillRect(0, barY3, w, 22*s);
    ctx.strokeStyle = aClr(p, 0.07); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, barY3); ctx.lineTo(w, barY3); ctx.stroke();
    ctx.fillStyle = aClr(p, 0.35); ctx.font = (5*s)+'px monospace';
    ctx.fillText('5 models', 10*s, barY3+14*s);
    ctx.textAlign = 'center'; ctx.fillStyle = 'rgba(0,220,100,0.35)'; ctx.fillText('fully local', w/2, barY3+14*s);
    ctx.textAlign = 'right'; ctx.fillStyle = aClr(p, 0.35); ctx.fillText('zero cloud', w-10*s, barY3+14*s); ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p, 0.15));
  }

  // ---- LLM ENCLAVE: Zero-trust AI isolation visual ----
  function drawLLMEnclave(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(220, 80, 60);

    // Dark bg with slight red tint
    var bg = ctx.createLinearGradient(0, 0, 0, h);
    if (p.light) { bg.addColorStop(0, '#ede8e7'); bg.addColorStop(1, '#e8e2e1'); }
    else { bg.addColorStop(0, '#100604'); bg.addColorStop(1, '#0a0403'); }
    ctx.fillStyle = bg; ctx.fillRect(0, 0, w, h);

    // Header bar
    var hGrad = ctx.createLinearGradient(0, 0, w, 0);
    hGrad.addColorStop(0, aClr(p, 0.07)); hGrad.addColorStop(1, aClr(p, 0.01));
    ctx.fillStyle = hGrad; ctx.fillRect(0, 0, w, 24*s);
    ctx.strokeStyle = aClr(p, 0.1); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 24*s); ctx.lineTo(w, 24*s); ctx.stroke();
    ctx.fillStyle = aClr(p, 0.55); ctx.fillRect(0, 0, 3*s, 24*s);
    ctx.fillStyle = tClr(p, 0.65); ctx.font = '500 '+(8*s)+'px Inter, sans-serif';
    ctx.fillText('LLM Enclave', 14*s, 16*s);
    ctx.fillStyle = aClr(p, 0.4); ctx.font = '200 '+(6*s)+'px Inter, sans-serif';
    ctx.textAlign = 'right'; ctx.fillText('ZERO-TRUST', w-14*s, 16*s); ctx.textAlign = 'left';

    // Concentric security rings
    var cx3 = w * 0.38, cy3 = h * 0.52;
    var rings = [
      {r: 78*s, label: 'Network Perimeter', c: '220,80,60', alpha: 0.06},
      {r: 60*s, label: 'Process Isolation', c: '220,130,60', alpha: 0.08},
      {r: 44*s, label: 'Memory Vault', c: '200,180,60', alpha: 0.1},
      {r: 28*s, label: 'LLM Core', c: '80,200,140', alpha: 0.14},
      {r: 14*s, label: 'Keys', c: '80,160,220', alpha: 0.2}
    ];
    for (var ri = 0; ri < rings.length; ri++) {
      var ring = rings[ri];
      ctx.strokeStyle = 'rgba('+ring.c+','+ring.alpha+')'; ctx.lineWidth = 1.5*s;
      ctx.beginPath(); ctx.arc(cx3, cy3, ring.r, 0, Math.PI*2); ctx.stroke();
      // Dashed segment
      ctx.setLineDash([3*s, 4*s]);
      ctx.strokeStyle = 'rgba('+ring.c+','+(ring.alpha*1.5)+')'; ctx.lineWidth = 0.5*s;
      ctx.beginPath(); ctx.arc(cx3, cy3, ring.r, -Math.PI*0.1, Math.PI*0.1); ctx.stroke();
      ctx.setLineDash([]);
      // Label at right of ring
      if (ri < 4) {
        ctx.fillStyle = 'rgba('+ring.c+','+(ring.alpha*3)+')'; ctx.font = '200 '+(3.5*s)+'px Inter, sans-serif';
        ctx.fillText(ring.label, cx3+ring.r+4*s, cy3-1*s);
      }
    }
    // Center dot
    ctx.fillStyle = 'rgba(80,200,140,0.4)';
    ctx.beginPath(); ctx.arc(cx3, cy3, 4*s, 0, Math.PI*2); ctx.fill();
    ctx.fillStyle = 'rgba(80,200,140,0.15)';
    ctx.beginPath(); ctx.arc(cx3, cy3, 8*s, 0, Math.PI*2); ctx.fill();

    // Right panel: access control matrix
    var panX = w * 0.62, panY = 30*s, panW = w*0.35, panH = h - 56*s;
    ctx.fillStyle = aClr(p, 0.02); ctx.fillRect(panX, panY, panW, panH);
    ctx.strokeStyle = aClr(p, 0.06); ctx.lineWidth = 0.5*s;
    ctx.strokeRect(panX, panY, panW, panH);
    ctx.fillStyle = aClr(p, 0.25); ctx.font = '200 '+(3.5*s)+'px Inter, sans-serif';
    ctx.fillText('ACCESS POLICY', panX+4*s, panY+9*s);
    var policies = [
      {name:'Network', val:'DENIED', c:'220,80,60'},
      {name:'Filesystem', val:'READ-ONLY', c:'220,150,60'},
      {name:'GPU', val:'ALLOWED', c:'80,200,140'},
      {name:'Memory', val:'ISOLATED', c:'80,160,220'},
      {name:'External', val:'BLOCKED', c:'220,80,60'},
      {name:'Logs', val:'ENCRYPTED', c:'80,200,140'}
    ];
    for (var pol = 0; pol < policies.length; pol++) {
      var py3 = panY + 14*s + pol*9.5*s;
      ctx.fillStyle = 'rgba('+policies[pol].c+',0.04)'; ctx.fillRect(panX+2*s, py3, panW-4*s, 8*s);
      ctx.fillStyle = tClr(p, 0.35); ctx.font = (3.2*s)+'px Inter, sans-serif';
      ctx.fillText(policies[pol].name, panX+5*s, py3+6*s);
      ctx.fillStyle = 'rgba('+policies[pol].c+',0.45)'; ctx.font = '600 '+(3.2*s)+'px monospace';
      ctx.textAlign = 'right'; ctx.fillText(policies[pol].val, panX+panW-4*s, py3+6*s); ctx.textAlign = 'left';
    }

    // Bottom bar
    var barY4 = h - 22*s;
    ctx.fillStyle = aClr(p, 0.04); ctx.fillRect(0, barY4, w, 22*s);
    ctx.strokeStyle = aClr(p, 0.08); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, barY4); ctx.lineTo(w, barY4); ctx.stroke();
    ctx.fillStyle = 'rgba(80,200,140,0.4)'; ctx.font = (5*s)+'px monospace';
    ctx.fillText('air-gapped', 10*s, barY4+14*s);
    ctx.textAlign = 'center'; ctx.fillStyle = aClr(p, 0.3); ctx.fillText('zero-trust', w/2, barY4+14*s);
    ctx.textAlign = 'right'; ctx.fillText('local GPU', w-10*s, barY4+14*s); ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p, 0.15));
  }

  // ---- SOVEREIGN TRADE ENGINE: Signal-to-exchange flow ----
  function drawSovereignTradeEngine(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(201, 168, 76);

    // Dark gold-tinted bg
    var bg = ctx.createLinearGradient(0, 0, 0, h);
    if (p.light) { bg.addColorStop(0, '#ece8dc'); bg.addColorStop(1, '#e6e2d6'); }
    else { bg.addColorStop(0, '#0d0b06'); bg.addColorStop(1, '#090807'); }
    ctx.fillStyle = bg; ctx.fillRect(0, 0, w, h);

    // Header bar
    var hGrad = ctx.createLinearGradient(0, 0, w, 0);
    hGrad.addColorStop(0, aClr(p, 0.08)); hGrad.addColorStop(1, aClr(p, 0.01));
    ctx.fillStyle = hGrad; ctx.fillRect(0, 0, w, 24*s);
    ctx.strokeStyle = aClr(p, 0.1); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 24*s); ctx.lineTo(w, 24*s); ctx.stroke();
    ctx.fillStyle = aClr(p, 0.5); ctx.fillRect(0, 0, 3*s, 24*s);
    ctx.fillStyle = tClr(p, 0.65); ctx.font = '500 '+(8*s)+'px Inter, sans-serif';
    ctx.fillText('Sovereign Trade Engine', 14*s, 16*s);
    ctx.fillStyle = 'rgba(220,80,60,0.45)'; ctx.font = '600 '+(5.5*s)+'px monospace';
    ctx.textAlign = 'right'; ctx.fillText('DRY RUN', w-14*s, 16*s); ctx.textAlign = 'left';

    // Signal flow pipeline
    var pipeNodes = [
      {label:'Market\nData', icon:'\u25CB', x:0.1, c:'80,160,220'},
      {label:'Signal\nEngine', icon:'\u25C7', x:0.3, c:'201,168,76'},
      {label:'profit-\ndesk', icon:'\u25D0', x:0.5, c:'201,168,76'},
      {label:'Master\nTrade Bot', icon:'\u25B7', x:0.7, c:'80,200,140'},
      {label:'Exchanges', icon:'\u25A0', x:0.9, c:'120,160,200'}
    ];
    var pipeY = h * 0.42;
    var nodeR = 14*s;
    // Connecting lines first
    for (var pni = 0; pni < pipeNodes.length-1; pni++) {
      var na = pipeNodes[pni], nb = pipeNodes[pni+1];
      var ax2 = na.x*w, bx2 = nb.x*w;
      // Signal pulse dots
      for (var pk = 1; pk <= 3; pk++) {
        var fx = ax2 + (bx2-ax2)*pk/4;
        ctx.fillStyle = aClr(p, 0.08+(pk*0.02));
        ctx.beginPath(); ctx.arc(fx, pipeY, 1.5*s, 0, Math.PI*2); ctx.fill();
      }
      ctx.strokeStyle = aClr(p, 0.1); ctx.lineWidth = 0.5*s;
      ctx.setLineDash([2*s, 2*s]);
      ctx.beginPath(); ctx.moveTo(ax2+nodeR, pipeY); ctx.lineTo(bx2-nodeR, pipeY); ctx.stroke();
      ctx.setLineDash([]);
    }
    // Nodes
    for (var pnj = 0; pnj < pipeNodes.length; pnj++) {
      var nd = pipeNodes[pnj];
      var nx = nd.x*w;
      ctx.fillStyle = 'rgba('+nd.c+',0.06)';
      ctx.beginPath(); ctx.arc(nx, pipeY, nodeR, 0, Math.PI*2); ctx.fill();
      ctx.strokeStyle = 'rgba('+nd.c+',0.2)'; ctx.lineWidth = 0.8*s;
      ctx.beginPath(); ctx.arc(nx, pipeY, nodeR, 0, Math.PI*2); ctx.stroke();
      ctx.fillStyle = 'rgba('+nd.c+',0.5)'; ctx.font = (6*s)+'px monospace';
      ctx.textAlign = 'center'; ctx.fillText(nd.icon, nx, pipeY+2*s);
      var labelLines = nd.label.split('\n');
      ctx.fillStyle = 'rgba('+nd.c+',0.35)'; ctx.font = '200 '+(3.5*s)+'px Inter, sans-serif';
      ctx.fillText(labelLines[0], nx, pipeY+nodeR+7*s);
      if (labelLines[1]) ctx.fillText(labelLines[1], nx, pipeY+nodeR+11*s);
      ctx.textAlign = 'left';
    }

    // Mini OHLC candlestick chart below pipeline
    var chartX = 10*s, chartY = pipeY + nodeR + 22*s;
    var chartW = w - 20*s, chartH = h - chartY - 30*s;
    ctx.fillStyle = aClr(p, 0.02); ctx.fillRect(chartX, chartY, chartW, chartH);
    ctx.strokeStyle = aClr(p, 0.05); ctx.lineWidth = 0.5*s;
    ctx.strokeRect(chartX, chartY, chartW, chartH);
    ctx.fillStyle = aClr(p, 0.2); ctx.font = '200 '+(3.5*s)+'px Inter, sans-serif';
    ctx.fillText('BTC/USDT — 4H', chartX+5*s, chartY+8*s);
    var candles = [
      {o:0.55,h:0.85,l:0.4,c:0.75,bull:true},
      {o:0.75,h:0.88,l:0.65,c:0.82,bull:true},
      {o:0.82,h:0.9,l:0.7,c:0.68,bull:false},
      {o:0.68,h:0.72,l:0.5,c:0.6,bull:false},
      {o:0.6,h:0.78,l:0.55,c:0.76,bull:true},
      {o:0.76,h:0.92,l:0.72,c:0.88,bull:true},
      {o:0.88,h:0.95,l:0.8,c:0.85,bull:false},
      {o:0.85,h:0.96,l:0.82,c:0.93,bull:true}
    ];
    var cW = (chartW - 16*s) / candles.length;
    for (var ci3 = 0; ci3 < candles.length; ci3++) {
      var cd = candles[ci3];
      var cxPos = chartX + 8*s + ci3*cW;
      var clr = cd.bull ? 'rgba(80,200,140,0.45)' : 'rgba(220,80,60,0.45)';
      // Wick
      ctx.strokeStyle = clr; ctx.lineWidth = 0.8*s;
      var hiY = chartY+chartH-4*s - cd.h*(chartH-14*s);
      var loY = chartY+chartH-4*s - cd.l*(chartH-14*s);
      ctx.beginPath(); ctx.moveTo(cxPos+cW/2, hiY); ctx.lineTo(cxPos+cW/2, loY); ctx.stroke();
      // Body
      var oY = chartY+chartH-4*s - cd.o*(chartH-14*s);
      var cY2 = chartY+chartH-4*s - cd.c*(chartH-14*s);
      ctx.fillStyle = clr; ctx.fillRect(cxPos+1*s, Math.min(oY,cY2), cW-2*s, Math.max(2*s,Math.abs(oY-cY2)));
    }

    // Bottom bar
    var barY5 = h - 22*s;
    ctx.fillStyle = aClr(p, 0.04); ctx.fillRect(0, barY5, w, 22*s);
    ctx.strokeStyle = aClr(p, 0.08); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, barY5); ctx.lineTo(w, barY5); ctx.stroke();
    ctx.fillStyle = aClr(p, 0.35); ctx.font = (5*s)+'px monospace';
    ctx.fillText('BTC / ETH / SOL', 10*s, barY5+14*s);
    ctx.textAlign = 'center'; ctx.fillStyle = 'rgba(220,80,60,0.4)'; ctx.fillText('DRY_RUN active', w/2, barY5+14*s);
    ctx.textAlign = 'right'; ctx.fillStyle = aClr(p, 0.35); ctx.fillText('multi-exchange', w-10*s, barY5+14*s); ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p, 0.15));
  }

  // ---- PRIVATE TAX: Document pipeline & finance dashboard ----
  function drawPrivateTax(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(100, 130, 190);

    // Cool blue-grey bg
    var bg = ctx.createLinearGradient(0, 0, 0, h);
    if (p.light) { bg.addColorStop(0, '#e5e7ed'); bg.addColorStop(1, '#e0e3ea'); }
    else { bg.addColorStop(0, '#070810'); bg.addColorStop(1, '#05060d'); }
    ctx.fillStyle = bg; ctx.fillRect(0, 0, w, h);

    // Header bar
    var hGrad = ctx.createLinearGradient(0, 0, w, 0);
    hGrad.addColorStop(0, aClr(p, 0.07)); hGrad.addColorStop(1, aClr(p, 0.01));
    ctx.fillStyle = hGrad; ctx.fillRect(0, 0, w, 24*s);
    ctx.strokeStyle = aClr(p, 0.09); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 24*s); ctx.lineTo(w, 24*s); ctx.stroke();
    ctx.fillStyle = aClr(p, 0.5); ctx.fillRect(0, 0, 3*s, 24*s);
    ctx.fillStyle = tClr(p, 0.65); ctx.font = '500 '+(8*s)+'px Inter, sans-serif';
    ctx.fillText('Private Tax & Finance', 14*s, 16*s);
    ctx.fillStyle = 'rgba(80,200,140,0.4)'; ctx.font = '200 '+(5.5*s)+'px monospace';
    ctx.textAlign = 'right'; ctx.fillText('ENCRYPTED', w-14*s, 16*s); ctx.textAlign = 'left';

    // Document pipeline stages
    var stages = [
      {name:'Raw Docs', icon:'\u2750', count:'847', c:'100,130,190'},
      {name:'Parsed', icon:'\u2B62', count:'812', c:'80,160,220'},
      {name:'Categorized', icon:'\u25A4', count:'798', c:'80,200,140'},
      {name:'LLM Tagged', icon:'\u25C9', count:'784', c:'201,168,76'},
      {name:'Tax Ready', icon:'\u2714', count:'741', c:'80,200,140'}
    ];
    var stageW = (w - 20*s) / stages.length;
    var stageY = 32*s;
    for (var sti = 0; sti < stages.length; sti++) {
      var stg = stages[sti];
      var stx = 10*s + sti*stageW;
      // Stage box
      ctx.fillStyle = 'rgba('+stg.c+',0.04)'; ctx.fillRect(stx+1*s, stageY, stageW-2*s, 32*s);
      ctx.strokeStyle = 'rgba('+stg.c+',0.1)'; ctx.lineWidth = 0.5*s;
      ctx.strokeRect(stx+1*s, stageY, stageW-2*s, 32*s);
      // Icon
      ctx.fillStyle = 'rgba('+stg.c+',0.4)'; ctx.font = (8*s)+'px monospace';
      ctx.textAlign = 'center'; ctx.fillText(stg.icon, stx+stageW/2, stageY+12*s);
      // Count
      ctx.fillStyle = 'rgba('+stg.c+',0.55)'; ctx.font = '600 '+(7*s)+'px Inter, sans-serif';
      ctx.fillText(stg.count, stx+stageW/2, stageY+22*s);
      // Name
      ctx.fillStyle = 'rgba('+stg.c+',0.3)'; ctx.font = '200 '+(3*s)+'px Inter, sans-serif';
      ctx.fillText(stg.name, stx+stageW/2, stageY+29*s);
      // Arrow
      if (sti < stages.length-1) {
        ctx.strokeStyle = aClr(p, 0.12); ctx.lineWidth = 0.5*s;
        ctx.beginPath(); ctx.moveTo(stx+stageW-1*s, stageY+16*s); ctx.lineTo(stx+stageW+1*s, stageY+16*s); ctx.stroke();
      }
    }
    ctx.textAlign = 'left';

    // Account summary table
    var tableY = stageY + 40*s;
    ctx.fillStyle = aClr(p, 0.02); ctx.fillRect(10*s, tableY, w-20*s, h-tableY-26*s);
    ctx.strokeStyle = aClr(p, 0.05); ctx.lineWidth = 0.5*s;
    ctx.strokeRect(10*s, tableY, w-20*s, h-tableY-26*s);
    ctx.fillStyle = aClr(p, 0.2); ctx.font = '200 '+(3.5*s)+'px Inter, sans-serif';
    ctx.fillText('ACCOUNT SUMMARY', 16*s, tableY+8*s);
    var accounts = [
      {name:'Checking', type:'Plaid', bal:'$12,450', chg:'+2.1%', c:'80,200,140'},
      {name:'Brokerage', type:'Manual', bal:'$84,200', chg:'+5.4%', c:'80,200,140'},
      {name:'Crypto', type:'API', bal:'$18,770', chg:'-1.2%', c:'220,80,60'},
      {name:'Savings', type:'Plaid', bal:'$31,100', chg:'+0.8%', c:'80,200,140'}
    ];
    var colW = (w-30*s)/4;
    for (var ai2 = 0; ai2 < accounts.length; ai2++) {
      var ac = accounts[ai2];
      var aX = 15*s + ai2*colW, aY = tableY+14*s;
      ctx.fillStyle = 'rgba('+ac.c+',0.03)'; ctx.fillRect(aX, aY, colW-4*s, h-tableY-40*s);
      ctx.strokeStyle = 'rgba('+ac.c+',0.07)'; ctx.lineWidth = 0.5*s;
      ctx.strokeRect(aX, aY, colW-4*s, h-tableY-40*s);
      ctx.fillStyle = tClr(p, 0.5); ctx.font = '400 '+(4*s)+'px Inter, sans-serif';
      ctx.fillText(ac.name, aX+4*s, aY+8*s);
      ctx.fillStyle = aClr(p, 0.25); ctx.font = (3*s)+'px monospace';
      ctx.fillText(ac.type, aX+4*s, aY+14*s);
      ctx.fillStyle = 'rgba('+ac.c+',0.55)'; ctx.font = '600 '+(6*s)+'px Inter, sans-serif';
      ctx.fillText(ac.bal, aX+4*s, aY+25*s);
      ctx.fillStyle = 'rgba('+ac.c+',0.4)'; ctx.font = (3.5*s)+'px monospace';
      ctx.fillText(ac.chg, aX+4*s, aY+32*s);
    }

    // Bottom bar
    var barY6 = h - 22*s;
    ctx.fillStyle = aClr(p, 0.04); ctx.fillRect(0, barY6, w, 22*s);
    ctx.strokeStyle = aClr(p, 0.07); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, barY6); ctx.lineTo(w, barY6); ctx.stroke();
    ctx.fillStyle = aClr(p, 0.35); ctx.font = (5*s)+'px monospace';
    ctx.fillText('847 docs', 10*s, barY6+14*s);
    ctx.textAlign = 'center'; ctx.fillText('4 accounts', w/2, barY6+14*s);
    ctx.textAlign = 'right'; ctx.fillText('5 tax years', w-10*s, barY6+14*s); ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p, 0.15));
  }

  // ---- PREDICTION MARKET EXECUTOR: Paused CLOB order book ----
  function drawPredictionMarketExecutor(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(140, 100, 200);

    // Dark purple bg — desaturated for paused state
    var bg = ctx.createLinearGradient(0, 0, 0, h);
    if (p.light) { bg.addColorStop(0, '#e9e7ef'); bg.addColorStop(1, '#e4e2ea'); }
    else { bg.addColorStop(0, '#0b090f'); bg.addColorStop(1, '#08070d'); }
    ctx.fillStyle = bg; ctx.fillRect(0, 0, w, h);

    // Header bar
    var hGrad = ctx.createLinearGradient(0, 0, w, 0);
    hGrad.addColorStop(0, aClr(p, 0.05)); hGrad.addColorStop(1, aClr(p, 0.01));
    ctx.fillStyle = hGrad; ctx.fillRect(0, 0, w, 24*s);
    ctx.strokeStyle = aClr(p, 0.07); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 24*s); ctx.lineTo(w, 24*s); ctx.stroke();
    ctx.fillStyle = aClr(p, 0.4); ctx.fillRect(0, 0, 3*s, 24*s);
    ctx.fillStyle = tClr(p, 0.45); ctx.font = '500 '+(8*s)+'px Inter, sans-serif';
    ctx.fillText('Prediction Market Executor', 14*s, 16*s);
    ctx.fillStyle = 'rgba(180,160,160,0.35)'; ctx.font = '600 '+(5.5*s)+'px monospace';
    ctx.textAlign = 'right'; ctx.fillText('PAUSED', w-14*s, 16*s); ctx.textAlign = 'left';

    // CLOB order book — left half
    var bookX = 10*s, bookY = 30*s;
    var bookW = (w - 20*s) * 0.48, bookH = h - bookY - 30*s;
    ctx.fillStyle = aClr(p, 0.02); ctx.fillRect(bookX, bookY, bookW, bookH);
    ctx.strokeStyle = aClr(p, 0.05); ctx.lineWidth = 0.5*s;
    ctx.strokeRect(bookX, bookY, bookW, bookH);
    ctx.fillStyle = aClr(p, 0.2); ctx.font = '200 '+(3.5*s)+'px Inter, sans-serif';
    ctx.fillText('ORDER BOOK — YES', bookX+4*s, bookY+8*s);
    // Bids (YES orders)
    var bids = [0.72,0.71,0.70,0.69,0.68,0.67];
    var rowH = (bookH-16*s)/12;
    for (var bi2 = 0; bi2 < bids.length; bi2++) {
      var barFill = (0.72-bids[bi2]+0.04)*3;
      ctx.fillStyle = 'rgba(80,200,140,0.08)';
      ctx.fillRect(bookX+2*s, bookY+12*s+bi2*rowH*2, bookW*barFill, rowH*1.6);
      ctx.fillStyle = 'rgba(80,200,140,0.25)'; ctx.font = (3.5*s)+'px monospace';
      ctx.fillText(bids[bi2].toFixed(2), bookX+4*s, bookY+12*s+bi2*rowH*2+rowH*1.2);
      ctx.textAlign = 'right';
      ctx.fillText(Math.floor(200-bi2*18)+' sh', bookX+bookW-4*s, bookY+12*s+bi2*rowH*2+rowH*1.2);
      ctx.textAlign = 'left';
    }
    // Asks (NO orders)
    var asks = [0.28,0.29,0.30,0.31,0.32,0.33];
    for (var ai3 = 0; ai3 < asks.length; ai3++) {
      var askFill = (asks[ai3]-0.27)*3;
      ctx.fillStyle = 'rgba(220,80,60,0.06)';
      ctx.fillRect(bookX+2*s, bookY+12*s+(ai3+6)*rowH*2, bookW*askFill, rowH*1.6);
      ctx.fillStyle = 'rgba(220,80,60,0.2)'; ctx.font = (3.5*s)+'px monospace';
      ctx.fillText(asks[ai3].toFixed(2), bookX+4*s, bookY+12*s+(ai3+6)*rowH*2+rowH*1.2);
      ctx.textAlign = 'right';
      ctx.fillText(Math.floor(150+ai3*22)+' sh', bookX+bookW-4*s, bookY+12*s+(ai3+6)*rowH*2+rowH*1.2);
      ctx.textAlign = 'left';
    }

    // Right side: market info and stats
    var infoX = bookX + bookW + 8*s, infoW = w - infoX - 10*s;
    ctx.fillStyle = aClr(p, 0.02); ctx.fillRect(infoX, bookY, infoW, bookH);
    ctx.strokeStyle = aClr(p, 0.05); ctx.lineWidth = 0.5*s;
    ctx.strokeRect(infoX, bookY, infoW, bookH);
    ctx.fillStyle = aClr(p, 0.2); ctx.font = '200 '+(3.5*s)+'px Inter, sans-serif';
    ctx.fillText('MARKET', infoX+4*s, bookY+8*s);
    ctx.fillStyle = tClr(p, 0.35); ctx.font = '400 '+(3.8*s)+'px Inter, sans-serif';
    ctx.fillText('BTC > $100k', infoX+4*s, bookY+16*s);
    ctx.fillText('by Dec 2025?', infoX+4*s, bookY+21*s);
    // Price
    ctx.fillStyle = 'rgba(80,200,140,0.35)'; ctx.font = '600 '+(10*s)+'px Inter, sans-serif';
    ctx.textAlign = 'center'; ctx.fillText('72¢', infoX+infoW/2, bookY+38*s); ctx.textAlign = 'left';
    // Stats
    var infoStats = [
      {l:'Vol', v:'$1.2M'}, {l:'Res.', v:'Dec 31'}, {l:'Pos.', v:'$0'}, {l:'P&L', v:'$0'}
    ];
    for (var is = 0; is < infoStats.length; is++) {
      var isy = bookY+50*s+is*12*s;
      ctx.fillStyle = aClr(p, 0.15); ctx.font = (3.2*s)+'px Inter, sans-serif';
      ctx.fillText(infoStats[is].l, infoX+4*s, isy);
      ctx.fillStyle = tClr(p, 0.3); ctx.font = '500 '+(4*s)+'px monospace';
      ctx.fillText(infoStats[is].v, infoX+4*s, isy+7*s);
    }

    // PAUSED overlay diagonal stripe
    ctx.save();
    ctx.globalAlpha = 0.035;
    ctx.strokeStyle = p.light ? '#6050a0' : '#8070c0'; ctx.lineWidth = 4*s;
    for (var ps = -h; ps < w+h; ps += 16*s) {
      ctx.beginPath(); ctx.moveTo(ps, 0); ctx.lineTo(ps+h, h); ctx.stroke();
    }
    ctx.restore();

    // Bottom bar
    var barY7 = h - 22*s;
    ctx.fillStyle = aClr(p, 0.03); ctx.fillRect(0, barY7, w, 22*s);
    ctx.strokeStyle = aClr(p, 0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, barY7); ctx.lineTo(w, barY7); ctx.stroke();
    ctx.fillStyle = 'rgba(180,160,160,0.3)'; ctx.font = (5*s)+'px monospace';
    ctx.fillText('Polymarket', 10*s, barY7+14*s);
    ctx.textAlign = 'center'; ctx.fillText('CLOB execution', w/2, barY7+14*s);
    ctx.textAlign = 'right'; ctx.fillText('paused', w-10*s, barY7+14*s); ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p, 0.1));
  }

  // ---- COMMAND CENTER: Unified sovereignty dashboard ----
  function drawCommandCenter(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(140, 160, 200);

    // Dark panel bg
    var bg = ctx.createLinearGradient(0, 0, 0, h);
    if (p.light) { bg.addColorStop(0, '#e4e6ec'); bg.addColorStop(1, '#dfe1e7'); }
    else { bg.addColorStop(0, '#08090c'); bg.addColorStop(1, '#050608'); }
    ctx.fillStyle = bg; ctx.fillRect(0, 0, w, h);

    // Subtle grid
    ctx.strokeStyle = aClr(p, 0.02); ctx.lineWidth = 0.5*s;
    for (var gx = 0; gx < w; gx += 20*s) { ctx.beginPath(); ctx.moveTo(gx, 0); ctx.lineTo(gx, h); ctx.stroke(); }
    for (var gy = 0; gy < h; gy += 20*s) { ctx.beginPath(); ctx.moveTo(0, gy); ctx.lineTo(w, gy); ctx.stroke(); }

    // Header bar
    var hGrad = ctx.createLinearGradient(0, 0, w, 0);
    hGrad.addColorStop(0, aClr(p, 0.06)); hGrad.addColorStop(1, aClr(p, 0.02));
    ctx.fillStyle = hGrad; ctx.fillRect(0, 0, w, 22*s);
    ctx.strokeStyle = aClr(p, 0.08); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 22*s); ctx.lineTo(w, 22*s); ctx.stroke();
    ctx.fillStyle = tClr(p, 0.6); ctx.font = '500 '+(7*s)+'px Inter, sans-serif';
    ctx.fillText('SOVEREIGN COMMAND CENTER', 12*s, 14.5*s);
    ctx.fillStyle = 'rgba(80,200,120,0.5)'; ctx.font = '400 '+(5*s)+'px Inter, sans-serif';
    ctx.textAlign = 'right'; ctx.fillText('ALL SYSTEMS NOMINAL', w-12*s, 14.5*s); ctx.textAlign = 'left';

    // 6-tab strip
    var tabY = 22*s, tabH = 12*s;
    ctx.fillStyle = aClr(p, 0.02); ctx.fillRect(0, tabY, w, tabH);
    ctx.strokeStyle = aClr(p, 0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, tabY+tabH); ctx.lineTo(w, tabY+tabH); ctx.stroke();
    var tabs = ['Overview','AI Cmd','Identity','Home','Comms','Gov'];
    var tabW = w / tabs.length;
    for (var ti = 0; ti < tabs.length; ti++) {
      if (ti === 0) {
        ctx.fillStyle = aClr(p, 0.06); ctx.fillRect(ti*tabW, tabY, tabW, tabH);
        ctx.fillStyle = aClr(p, 0.6); ctx.fillRect(ti*tabW, tabY+tabH-1.5*s, tabW, 1.5*s);
      }
      ctx.fillStyle = ti === 0 ? tClr(p, 0.55) : tClr(p, 0.2);
      ctx.font = (ti === 0 ? '500 ' : '300 ')+(4.5*s)+'px Inter, sans-serif';
      ctx.textAlign = 'center'; ctx.fillText(tabs[ti], ti*tabW+tabW/2, tabY+8.5*s);
    }
    ctx.textAlign = 'left';

    // Posture meter cards (5 across)
    var mY = 40*s, mH = 28*s, mW = (w - 60*s) / 5;
    var meters = [
      {label:'AI GOV', val: 92, c:'80,200,120'},
      {label:'IDENTITY', val: 88, c:'80,200,120'},
      {label:'NETWORK', val: 95, c:'80,200,120'},
      {label:'ENCRYPT', val: 100, c:'80,200,120'},
      {label:'CLOUD', val: 0, c:'140,160,200'}
    ];
    for (var mi = 0; mi < meters.length; mi++) {
      var mx = 10*s + mi*(mW+10*s);
      ctx.fillStyle = aClr(p, 0.025); ctx.fillRect(mx, mY, mW, mH);
      ctx.strokeStyle = aClr(p, 0.06); ctx.lineWidth = 0.5*s; ctx.strokeRect(mx, mY, mW, mH);
      ctx.fillStyle = tClr(p, 0.3); ctx.font = '500 '+(3.5*s)+'px Inter, sans-serif';
      ctx.fillText(meters[mi].label, mx+4*s, mY+10*s);
      // Bar
      var barX = mx+4*s, barW = mW-8*s, barH2 = 4*s, barY2 = mY+16*s;
      ctx.fillStyle = aClr(p, 0.04); ctx.fillRect(barX, barY2, barW, barH2);
      ctx.fillStyle = 'rgba('+meters[mi].c+',0.35)'; ctx.fillRect(barX, barY2, barW*(meters[mi].val/100), barH2);
      ctx.fillStyle = 'rgba('+meters[mi].c+',0.45)'; ctx.font = '600 '+(3.5*s)+'px Inter, sans-serif';
      ctx.textAlign = 'right'; ctx.fillText(meters[mi].val+'%', mx+mW-4*s, mY+10*s); ctx.textAlign = 'left';
    }

    // Product family cards (3x2 grid)
    var families = ['AI Command','Identity & Trust','Home Control','Communications','Governance','Session Atlas'];
    var cardW = (w - 30*s) / 3, cardH2 = 20*s, cardY = 74*s;
    for (var fi = 0; fi < families.length; fi++) {
      var col = fi % 3, row = Math.floor(fi / 3);
      var fx = 10*s + col*(cardW+5*s), fy = cardY + row*(cardH2+5*s);
      ctx.fillStyle = aClr(p, 0.025); ctx.fillRect(fx, fy, cardW, cardH2);
      ctx.strokeStyle = aClr(p, 0.05); ctx.lineWidth = 0.5*s; ctx.strokeRect(fx, fy, cardW, cardH2);
      ctx.fillStyle = 'rgba(80,200,120,0.3)';
      ctx.beginPath(); ctx.arc(fx+6*s, fy+cardH2/2, 2*s, 0, Math.PI*2); ctx.fill();
      ctx.fillStyle = tClr(p, 0.35); ctx.font = '400 '+(3.8*s)+'px Inter, sans-serif';
      ctx.fillText(families[fi], fx+12*s, fy+cardH2/2+1.5*s);
    }

    // Event feed at bottom
    var feedY = h - 36*s;
    ctx.fillStyle = aClr(p, 0.03); ctx.fillRect(0, feedY, w, 36*s);
    ctx.strokeStyle = aClr(p, 0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, feedY); ctx.lineTo(w, feedY); ctx.stroke();
    ctx.fillStyle = tClr(p, 0.25); ctx.font = '500 '+(4*s)+'px Inter, sans-serif';
    ctx.fillText('LIVE EVENT FEED', 10*s, feedY+10*s);
    var events = ['Tier 0 auto-approved','Ollama inference OK','Session state: Advisory','Hub sync complete'];
    for (var ei = 0; ei < events.length; ei++) {
      ctx.fillStyle = tClr(p, 0.18); ctx.font = '300 '+(3.2*s)+'px monospace';
      ctx.fillText(events[ei], 10*s, feedY+18*s+ei*5*s);
    }

    drawCorners(ctx, w, h, 14*s, aClr(p, 0.12));
  }

  // ---- AI ORCHESTRATOR: Multi-model governance flow ----
  function drawAIOrchestrator(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(60, 180, 200);

    // Dark bg
    var bg = ctx.createLinearGradient(0, 0, 0, h);
    if (p.light) { bg.addColorStop(0, '#e2ecee'); bg.addColorStop(1, '#dde8ea'); }
    else { bg.addColorStop(0, '#060b0c'); bg.addColorStop(1, '#04080a'); }
    ctx.fillStyle = bg; ctx.fillRect(0, 0, w, h);

    // Grid
    ctx.strokeStyle = aClr(p, 0.02); ctx.lineWidth = 0.5*s;
    for (var gx = 0; gx < w; gx += 20*s) { ctx.beginPath(); ctx.moveTo(gx, 0); ctx.lineTo(gx, h); ctx.stroke(); }
    for (var gy = 0; gy < h; gy += 20*s) { ctx.beginPath(); ctx.moveTo(0, gy); ctx.lineTo(w, gy); ctx.stroke(); }

    // Header
    ctx.fillStyle = aClr(p, 0.05); ctx.fillRect(0, 0, w, 22*s);
    ctx.strokeStyle = aClr(p, 0.08); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 22*s); ctx.lineTo(w, 22*s); ctx.stroke();
    ctx.fillStyle = tClr(p, 0.6); ctx.font = '500 '+(7*s)+'px Inter, sans-serif';
    ctx.fillText('AI ORCHESTRATOR', 12*s, 14.5*s);
    ctx.fillStyle = aClr(p, 0.4); ctx.font = '400 '+(5*s)+'px Inter, sans-serif';
    ctx.textAlign = 'right'; ctx.fillText('LOCAL-FIRST', w-12*s, 14.5*s); ctx.textAlign = 'left';

    // Posture meters (3 across)
    var pmY = 28*s, pmH = 22*s, pmW = (w - 40*s) / 3;
    var pms = [{l:'INFERENCE',v:'Local'},{l:'POLICY',v:'Active'},{l:'AUDIT',v:'Chain'}];
    for (var pi = 0; pi < pms.length; pi++) {
      var px = 10*s + pi*(pmW+10*s);
      ctx.fillStyle = aClr(p, 0.025); ctx.fillRect(px, pmY, pmW, pmH);
      ctx.strokeStyle = aClr(p, 0.06); ctx.lineWidth = 0.5*s; ctx.strokeRect(px, pmY, pmW, pmH);
      ctx.fillStyle = tClr(p, 0.25); ctx.font = '500 '+(3.5*s)+'px Inter, sans-serif';
      ctx.fillText(pms[pi].l, px+5*s, pmY+9*s);
      ctx.fillStyle = aClr(p, 0.5); ctx.font = '600 '+(5*s)+'px Inter, sans-serif';
      ctx.fillText(pms[pi].v, px+5*s, pmY+17*s);
    }

    // Flow diagram: 6 steps with arrows
    var flowY = 56*s, flowH = 16*s;
    var steps = ['Request','Classify','Route','Enforce','Execute','Audit'];
    var stepW = (w - 20*s - (steps.length-1)*8*s) / steps.length;
    for (var si = 0; si < steps.length; si++) {
      var sx = 10*s + si*(stepW+8*s);
      ctx.fillStyle = aClr(p, 0.04); ctx.fillRect(sx, flowY, stepW, flowH);
      ctx.strokeStyle = aClr(p, 0.1); ctx.lineWidth = 0.5*s; ctx.strokeRect(sx, flowY, stepW, flowH);
      ctx.fillStyle = tClr(p, 0.35); ctx.font = '400 '+(3.5*s)+'px Inter, sans-serif';
      ctx.textAlign = 'center'; ctx.fillText(steps[si], sx+stepW/2, flowY+flowH/2+1.5*s);
      // Arrow
      if (si < steps.length - 1) {
        var ax = sx + stepW + 1*s, ay = flowY + flowH/2;
        ctx.fillStyle = aClr(p, 0.25);
        ctx.beginPath(); ctx.moveTo(ax, ay-2*s); ctx.lineTo(ax+5*s, ay); ctx.lineTo(ax, ay+2*s); ctx.closePath(); ctx.fill();
      }
    }
    ctx.textAlign = 'left';

    // Component cards (2x3 grid)
    var components = ['System Router','LLM Enclave','LocalClaude','Auton','Governance','Session Atlas'];
    var ccW = (w - 30*s) / 3, ccH = 18*s, ccY = 78*s;
    for (var ci = 0; ci < components.length; ci++) {
      var col = ci % 3, row = Math.floor(ci / 3);
      var cx2 = 10*s + col*(ccW+5*s), cy2 = ccY + row*(ccH+4*s);
      ctx.fillStyle = aClr(p, 0.025); ctx.fillRect(cx2, cy2, ccW, ccH);
      ctx.strokeStyle = aClr(p, 0.05); ctx.lineWidth = 0.5*s; ctx.strokeRect(cx2, cy2, ccW, ccH);
      ctx.fillStyle = 'rgba(60,180,200,0.3)';
      ctx.beginPath(); ctx.arc(cx2+6*s, cy2+ccH/2, 2*s, 0, Math.PI*2); ctx.fill();
      ctx.fillStyle = tClr(p, 0.32); ctx.font = '400 '+(3.5*s)+'px Inter, sans-serif';
      ctx.fillText(components[ci], cx2+12*s, cy2+ccH/2+1.5*s);
    }

    // Model routing table
    var tableY = h - 38*s;
    ctx.fillStyle = aClr(p, 0.03); ctx.fillRect(0, tableY, w, 38*s);
    ctx.strokeStyle = aClr(p, 0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, tableY); ctx.lineTo(w, tableY); ctx.stroke();
    ctx.fillStyle = tClr(p, 0.25); ctx.font = '500 '+(4*s)+'px Inter, sans-serif';
    ctx.fillText('MODEL ROUTING', 10*s, tableY+10*s);
    var models = ['Qwen 2.5  Local','Llama 3   Local','Claude    Cloud','GPT-compat Cloud'];
    for (var mdi = 0; mdi < models.length; mdi++) {
      ctx.fillStyle = tClr(p, 0.18); ctx.font = '300 '+(3*s)+'px monospace';
      ctx.fillText(models[mdi], 10*s, tableY+18*s+mdi*5*s);
      var isLocal = mdi < 2;
      ctx.fillStyle = isLocal ? 'rgba(80,200,120,0.3)' : 'rgba(220,180,60,0.3)';
      ctx.beginPath(); ctx.arc(w-20*s, tableY+17*s+mdi*5*s, 2*s, 0, Math.PI*2); ctx.fill();
    }

    drawCorners(ctx, w, h, 14*s, aClr(p, 0.12));
  }

  // ---- GOVERNANCE STANDARDS: Tier and session state framework ----
  function drawGovernance(canvas) {
    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;
    var p = pal(200, 170, 60);

    // Dark bg
    var bg = ctx.createLinearGradient(0, 0, 0, h);
    if (p.light) { bg.addColorStop(0, '#edebe2'); bg.addColorStop(1, '#e8e6dd'); }
    else { bg.addColorStop(0, '#0c0b06'); bg.addColorStop(1, '#090805'); }
    ctx.fillStyle = bg; ctx.fillRect(0, 0, w, h);

    // Grid
    ctx.strokeStyle = aClr(p, 0.015); ctx.lineWidth = 0.5*s;
    for (var gx = 0; gx < w; gx += 20*s) { ctx.beginPath(); ctx.moveTo(gx, 0); ctx.lineTo(gx, h); ctx.stroke(); }
    for (var gy = 0; gy < h; gy += 20*s) { ctx.beginPath(); ctx.moveTo(0, gy); ctx.lineTo(w, gy); ctx.stroke(); }

    // Header
    ctx.fillStyle = aClr(p, 0.04); ctx.fillRect(0, 0, w, 22*s);
    ctx.strokeStyle = aClr(p, 0.08); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, 22*s); ctx.lineTo(w, 22*s); ctx.stroke();
    ctx.fillStyle = tClr(p, 0.6); ctx.font = '500 '+(7*s)+'px Inter, sans-serif';
    ctx.fillText('GOVERNANCE STANDARDS', 12*s, 14.5*s);
    ctx.fillStyle = aClr(p, 0.4); ctx.font = '400 '+(5*s)+'px Inter, sans-serif';
    ctx.textAlign = 'right'; ctx.fillText('37K+ LINES', w-12*s, 14.5*s); ctx.textAlign = 'left';

    // 5-tier approval table
    var tiers = [
      {name:'Tier 0', desc:'Auto-Approve', c:'80,200,120'},
      {name:'Tier 1', desc:'Notify', c:'100,180,220'},
      {name:'Tier 2', desc:'Confirm', c:'200,170,60'},
      {name:'Tier 3', desc:'Elevated', c:'200,80,80'},
      {name:'Tier 4', desc:'Emergency', c:'160,100,200'}
    ];
    var tierY = 28*s, tierH = 10*s;
    ctx.fillStyle = tClr(p, 0.25); ctx.font = '500 '+(4*s)+'px Inter, sans-serif';
    ctx.fillText('APPROVAL TIERS', 10*s, tierY+7*s);
    for (var tii = 0; tii < tiers.length; tii++) {
      var ty = tierY + 12*s + tii*tierH;
      // Color bar left edge
      ctx.fillStyle = 'rgba('+tiers[tii].c+',0.25)'; ctx.fillRect(10*s, ty, 3*s, tierH-2*s);
      ctx.fillStyle = aClr(p, 0.02); ctx.fillRect(13*s, ty, w-23*s, tierH-2*s);
      ctx.strokeStyle = aClr(p, 0.04); ctx.lineWidth = 0.5*s; ctx.strokeRect(13*s, ty, w-23*s, tierH-2*s);
      ctx.fillStyle = 'rgba('+tiers[tii].c+',0.45)'; ctx.font = '500 '+(3.5*s)+'px Inter, sans-serif';
      ctx.fillText(tiers[tii].name, 18*s, ty+6.5*s);
      ctx.fillStyle = tClr(p, 0.25); ctx.font = '300 '+(3.5*s)+'px Inter, sans-serif';
      ctx.fillText(tiers[tii].desc, 50*s, ty+6.5*s);
    }

    // Session states (4x2 grid)
    var states = ['Advisory','Read-Only','Guided','Standard','Approved','Elevated','Restricted','Emergency'];
    var stW = (w - 30*s) / 4, stH = 14*s, stY = tierY + 12*s + 5*tierH + 6*s;
    ctx.fillStyle = tClr(p, 0.25); ctx.font = '500 '+(4*s)+'px Inter, sans-serif';
    ctx.fillText('SESSION STATES', 10*s, stY - 2*s);
    for (var sti = 0; sti < states.length; sti++) {
      var col = sti % 4, row = Math.floor(sti / 4);
      var stx = 10*s + col*(stW+3.3*s), sty = stY + 4*s + row*(stH+3*s);
      ctx.fillStyle = aClr(p, 0.025); ctx.fillRect(stx, sty, stW, stH);
      ctx.strokeStyle = aClr(p, 0.05); ctx.lineWidth = 0.5*s; ctx.strokeRect(stx, sty, stW, stH);
      ctx.fillStyle = tClr(p, 0.28); ctx.font = '400 '+(3.2*s)+'px Inter, sans-serif';
      ctx.textAlign = 'center'; ctx.fillText(states[sti], stx+stW/2, sty+stH/2+1.2*s);
    }
    ctx.textAlign = 'left';

    // Bottom stats bar
    var barY3 = h - 20*s;
    ctx.fillStyle = aClr(p, 0.03); ctx.fillRect(0, barY3, w, 20*s);
    ctx.strokeStyle = aClr(p, 0.06); ctx.lineWidth = 0.5*s;
    ctx.beginPath(); ctx.moveTo(0, barY3); ctx.lineTo(w, barY3); ctx.stroke();
    ctx.fillStyle = aClr(p, 0.35); ctx.font = '500 '+(4*s)+'px monospace';
    ctx.fillText('5 tiers', 10*s, barY3+13*s);
    ctx.textAlign = 'center'; ctx.fillText('8 states', w/2, barY3+13*s);
    ctx.textAlign = 'right'; ctx.fillText('30 docs', w-10*s, barY3+13*s); ctx.textAlign = 'left';

    drawCorners(ctx, w, h, 14*s, aClr(p, 0.1));
  }

  // ===== DISPATCH =====
  var renderers = {
    booking: drawBooking, trading: drawTrading, profitdesk: drawProfitDesk,
    rts: drawRTS, platformer: drawPlatformer, messenger: drawMessenger,
    polymarket: drawPolymarket, galleon: drawGalleon,
    bookmarkbot: drawBookmarkBot, homehub: drawHomeHub,
    projecthub: drawProjectHub, fisher13: drawFisher13,
    noco: drawNoCo, marketdash: drawMarketDash, auton: drawAuton,
    'operator-console': drawOperatorConsole,
    'session-atlas': drawSessionAtlas,
    'private-intelligence': drawPrivateIntelligence,
    'llm-enclave': drawLLMEnclave,
    'sovereign-trade-engine': drawSovereignTradeEngine,
    'private-tax': drawPrivateTax,
    'prediction-market-executor': drawPredictionMarketExecutor,
    'command-center': drawCommandCenter,
    'ai-orchestrator': drawAIOrchestrator,
    'governance': drawGovernance
  };

  function initProjectCanvases() {
    document.querySelectorAll('canvas[data-project]').forEach(function(cv) {
      var key = cv.getAttribute('data-project');
      if (renderers[key]) renderers[key](cv);
    });
  }

  // ===== FOOTER MOTTO: Latin ↔ English scramble =====
  function initFooterMotto() {
    var latinEl = document.getElementById('footerMottoLatin');
    var enEl = document.getElementById('footerMottoEn');
    if (!latinEl || !enEl) return;
    var glitchChars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz';
    var latinText = 'Mens clara in tenebris';
    var enText = 'A clear mind in darkness';
    var showing = 'latin';
    function scramble(fromEl, toEl, targetText, callback) {
      fromEl.style.display = 'none';
      toEl.style.display = 'inline';
      var len = targetText.length;
      var iterations = 0;
      var maxIterations = 14;
      var interval = setInterval(function() {
        var out = '';
        for (var i = 0; i < len; i++) {
          if (targetText[i] === ' ') {
            out += ' ';
          } else if (iterations >= maxIterations - 4 && Math.random() > 0.45) {
            out += targetText[i];
          } else {
            out += glitchChars[Math.floor(Math.random() * glitchChars.length)];
          }
        }
        toEl.textContent = out;
        iterations++;
        if (iterations >= maxIterations) {
          clearInterval(interval);
          toEl.textContent = targetText;
          if (callback) callback();
        }
      }, 55);
    }
    function cycle() {
      if (showing === 'latin') {
        scramble(latinEl, enEl, enText, function() {
          showing = 'en';
          setTimeout(function() {
            scramble(enEl, latinEl, latinText, function() { showing = 'latin'; });
          }, 3500);
        });
      }
    }
    // First cycle after 8s, then every 14-20s — subtle, not distracting
    setTimeout(function() {
      cycle();
      setInterval(cycle, 14000 + Math.random() * 6000);
    }, 8000);
  }

  // Init
  window.addEventListener('load', function() {
    initProjectCanvases();
    initFooterMotto();
  });
  var resizeTimer;
  window.addEventListener('resize', function() {
    clearTimeout(resizeTimer);
    resizeTimer = setTimeout(function() { initProjectCanvases(); }, 300);
  });

  // Lazy-load concept iframes when gallery scrolls into view
  var conceptGallery = document.querySelector('.concept-gallery');
  if (conceptGallery) {
    var conceptLoaded = false;
    var conceptObserver = new IntersectionObserver(function(entries) {
      if (entries[0].isIntersecting && !conceptLoaded) {
        conceptLoaded = true;
        var thumbs = document.querySelectorAll('.concept-thumb[data-src]');
        thumbs.forEach(function(thumb) {
          var iframe = document.createElement('iframe');
          iframe.src = thumb.getAttribute('data-src');
          iframe.loading = 'lazy';
          iframe.tabIndex = -1;
          iframe.setAttribute('aria-hidden', 'true');
          iframe.setAttribute('sandbox', 'allow-same-origin');
          var placeholder = thumb.querySelector('.concept-placeholder');
          if (placeholder) placeholder.remove();
          thumb.appendChild(iframe);
        });
        conceptObserver.disconnect();
      }
    }, { rootMargin: '200px' });
    conceptObserver.observe(conceptGallery);
  }

})();
