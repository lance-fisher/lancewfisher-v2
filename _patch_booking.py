with open('D:/ProjectsHome/FisherSovereign/lancewfisher-v2/deploy/main.js', 'r', encoding='utf-8') as f:
    lines = f.readlines()

new_fn = (
    '  // ---- BOOKING: Phone mockup with Serenity Wellness App ----\n'
    '  function drawBooking(canvas) {\n'
    '    var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;\n'
    '    drawBg(ctx, w, h, 184, 156, 92);\n'
    '    var p = pal(184, 156, 92);\n'
    '\n'
    '    // Subtle grid pattern\n'
    '    ctx.strokeStyle = aClr(p,0.02); ctx.lineWidth = 0.5*s;\n'
    '    for (var gi = 0; gi < w; gi += 20*s) { ctx.beginPath(); ctx.moveTo(gi, 0); ctx.lineTo(gi, h); ctx.stroke(); }\n'
    '    for (var gj = 0; gj < h; gj += 20*s) { ctx.beginPath(); ctx.moveTo(0, gj); ctx.lineTo(w, gj); ctx.stroke(); }\n'
    '\n'
    '    // Phone frame\n'
    '    var pw = 140*s, ph = 240*s;\n'
    '    var px = w*0.12, py = (h - ph)/2;\n'
    '\n'
    '    // Phone shadow\n'
    "    ctx.save(); ctx.shadowColor = (p.light ? 'rgba(26,26,28,0.16)' : 'rgba(0,0,0,0.4)'); ctx.shadowBlur = 30*s; ctx.shadowOffsetX = 6*s; ctx.shadowOffsetY = 8*s;\n"
    '    roundRect(ctx, px, py, pw, ph, 18*s);\n'
    "    ctx.fillStyle = p.light ? '#b8b9bc' : '#1a1a1c'; ctx.fill();\n"
    '    ctx.restore();\n'
    '\n'
    '    // Phone bezel\n'
    '    roundRect(ctx, px, py, pw, ph, 18*s);\n'
    '    ctx.strokeStyle = aClr(p,0.3); ctx.lineWidth = 1.5*s; ctx.stroke();\n'
    '\n'
    '    // Dynamic island\n'
    '    roundRect(ctx, px + pw*0.28, py+4*s, pw*0.44, 10*s, 5*s);\n'
    "    ctx.fillStyle = p.light ? '#8a8b8e' : '#000'; ctx.fill();\n"
    '\n'
    '    // Screen area\n'
    '    var sx = px + 5*s, sy = py + 18*s, sw = pw - 10*s, sh = ph - 26*s;\n'
    '    roundRect(ctx, sx, sy, sw, sh, 4*s);\n'
    "    ctx.fillStyle = '#faf6f0'; ctx.fill();\n"
    '\n'
    '    // Status bar\n'
    "    ctx.fillStyle = '#efe9e0'; ctx.fillRect(sx, sy, sw, 14*s);\n"
    "    ctx.fillStyle = 'rgba(80,60,30,0.5)'; ctx.font = '500 '+(6*s)+'px Inter, sans-serif';\n"
    "    ctx.textAlign = 'center'; ctx.fillText('9:41', sx+sw/2, sy+10*s);\n"
    "    ctx.textAlign = 'left';\n"
    "    ctx.fillStyle = 'rgba(80,60,30,0.3)'; ctx.font = (5*s)+'px Inter, sans-serif';\n"
    "    ctx.fillText('LTE', sx+4*s, sy+10*s);\n"
    '\n'
    '    // App header - Serenity branding\n'
    "    ctx.fillStyle = '#f4ede4'; ctx.fillRect(sx, sy+14*s, sw, 24*s);\n"
    "    ctx.fillStyle = '#8b7b60'; ctx.font = '500 '+(9*s)+'px \"Cormorant Garamond\", serif';\n"
    "    ctx.textAlign = 'center'; ctx.fillText('Serenity', sx+sw/2, sy+29*s);\n"
    "    ctx.fillStyle = '#b0a088'; ctx.font = '300 '+(4*s)+'px Inter, sans-serif';\n"
    "    ctx.fillText('WELLNESS CLIENT APP', sx+sw/2, sy+36*s);\n"
    '\n'
    '    // Promo banner - gradient card\n'
    '    var bannerY = sy+42*s, bannerH = 44*s;\n'
    '    roundRect(ctx, sx+5*s, bannerY, sw-10*s, bannerH, 5*s);\n'
    '    var promoGrad = ctx.createLinearGradient(sx+5*s, bannerY, sx+sw-5*s, bannerY+bannerH);\n'
    "    promoGrad.addColorStop(0, '#c8a96e'); promoGrad.addColorStop(1, '#8b6740');\n"
    '    ctx.fillStyle = promoGrad; ctx.fill();\n'
    "    ctx.textAlign = 'left';\n"
    "    ctx.fillStyle = 'rgba(255,255,255,0.55)'; ctx.font = '400 '+(4*s)+'px Inter, sans-serif';\n"
    "    ctx.fillText('NEW CLIENT OFFER', sx+11*s, bannerY+11*s);\n"
    "    ctx.fillStyle = '#fff'; ctx.font = '500 '+(7.5*s)+'px \"Cormorant Garamond\", serif';\n"
    "    ctx.fillText('Welcome', sx+11*s, bannerY+23*s);\n"
    "    ctx.fillText('Consultation', sx+11*s, bannerY+32*s);\n"
    "    ctx.fillStyle = 'rgba(255,255,255,0.65)'; ctx.font = '300 '+(4*s)+'px Inter, sans-serif';\n"
    "    ctx.fillText('Complimentary with first visit', sx+11*s, bannerY+41*s);\n"
    '\n'
    '    // Service category cards - 2x2 grid\n'
    "    var services = [{t:'Facials'}, {t:'Laser'}, {t:'Injectables'}, {t:'Body'}];\n"
    '    var cardW = (sw - 16*s) / 2, cardH = 22*s;\n'
    '    for (var si = 0; si < 4; si++) {\n'
    '      var ccx = sx + 5*s + (si % 2) * (cardW + 6*s);\n'
    '      var ccy = sy + 92*s + Math.floor(si / 2) * (cardH + 5*s);\n'
    '      roundRect(ctx, ccx, ccy, cardW, cardH, 4*s);\n'
    "      ctx.fillStyle = '#f0e9df'; ctx.fill();\n"
    '      ctx.strokeStyle = aClr(p,0.12); ctx.lineWidth = 0.5*s; ctx.stroke();\n'
    "      ctx.fillStyle = '#6b5e4a'; ctx.font = '400 '+(5.5*s)+'px Inter, sans-serif';\n"
    "      ctx.textAlign = 'center'; ctx.fillText(services[si].t, ccx+cardW/2, ccy+14*s);\n"
    '    }\n'
    '\n'
    '    // Provider row with initials\n'
    "    ctx.fillStyle = '#7a6c55'; ctx.font = '500 '+(5*s)+'px Inter, sans-serif';\n"
    "    ctx.textAlign = 'left'; ctx.fillText('Our Providers', sx+7*s, sy+150*s);\n"
    "    var provColors = ['#c4b09a','#b8a894','#a89a88','#beae9c'];\n"
    "    var provInitials = ['AC','TB','JR','ML'];\n"
    '    for (var pi = 0; pi < 4; pi++) {\n'
    '      var pcx = sx + 16*s + pi*24*s, pcy = sy+162*s;\n'
    '      ctx.beginPath(); ctx.arc(pcx, pcy, 10*s, 0, Math.PI*2);\n'
    '      ctx.fillStyle = provColors[pi]; ctx.fill();\n'
    '      ctx.strokeStyle = aClr(p,0.25); ctx.lineWidth = 0.8*s; ctx.stroke();\n'
    "      ctx.fillStyle = 'rgba(255,255,255,0.85)'; ctx.font = '500 '+(5*s)+'px Inter, sans-serif';\n"
    "      ctx.textAlign = 'center'; ctx.fillText(provInitials[pi], pcx, pcy+2*s);\n"
    '    }\n'
    '\n'
    '    // Book Now CTA\n'
    "    ctx.textAlign = 'left';\n"
    '    roundRect(ctx, sx+5*s, sy+182*s, sw-10*s, 18*s, 6*s);\n'
    '    var btnGrad = ctx.createLinearGradient(0, sy+182*s, 0, sy+200*s);\n'
    "    btnGrad.addColorStop(0, aClr(p,0.9)); btnGrad.addColorStop(1, 'rgba(160,136,72,0.9)');\n"
    '    ctx.fillStyle = btnGrad; ctx.fill();\n'
    "    ctx.fillStyle = '#fff'; ctx.font = '600 '+(6*s)+'px Inter, sans-serif';\n"
    "    ctx.textAlign = 'center'; ctx.fillText('Book Appointment', sx+sw/2, sy+194*s);\n"
    '\n'
    '    // Tab bar\n'
    "    ctx.fillStyle = '#f4ede4'; ctx.fillRect(sx, sy+sh-18*s, sw, 18*s);\n"
    '    ctx.strokeStyle = aClr(p,0.08); ctx.lineWidth = 0.5*s;\n'
    '    ctx.beginPath(); ctx.moveTo(sx, sy+sh-18*s); ctx.lineTo(sx+sw, sy+sh-18*s); ctx.stroke();\n'
    "    var tabs = ['Home','Services','Book','Profile'];\n"
    "    ctx.font = (4.5*s)+'px Inter, sans-serif';\n"
    '    for (var ti = 0; ti < tabs.length; ti++) {\n'
    '      var tx = sx + sw/(tabs.length*2) + ti*(sw/tabs.length);\n'
    "      ctx.fillStyle = ti === 0 ? '#8b7b60' : '#b8a898';\n"
    '      ctx.fillText(tabs[ti], tx, sy+sh-6*s);\n'
    '    }\n'
    '\n'
    '    // === Right side: Serenity feature showcase ===\n'
    '    var rx = w*0.5;\n'
    "    ctx.textAlign = 'left';\n"
    "    ctx.fillStyle = aClr(p,0.4); ctx.font = '200 '+(8*s)+'px Inter, sans-serif';\n"
    "    ctx.fillText('WELLNESS CLIENT APP', rx, h*0.18);\n"
    "    ctx.fillStyle = tClr(p,0.75); ctx.font = '300 '+(22*s)+'px \"Cormorant Garamond\", serif';\n"
    "    ctx.fillText('Serenity', rx, h*0.34);\n"
    '\n'
    '    // Thin rule\n'
    '    ctx.fillStyle = aClr(p,0.2); ctx.fillRect(rx, h*0.38, 40*s, 1);\n'
    '\n'
    '    // Mini upcoming appointment card\n'
    '    roundRect(ctx, rx, h*0.42, 120*s, 40*s, 6*s);\n'
    "    ctx.fillStyle = p.light ? 'rgba(255,255,255,0.65)' : 'rgba(255,255,255,0.07)'; ctx.fill();\n"
    '    ctx.strokeStyle = aClr(p,0.12); ctx.lineWidth = 0.5*s; ctx.stroke();\n'
    "    ctx.fillStyle = aClr(p,0.38); ctx.font = '400 '+(4.5*s)+'px Inter, sans-serif';\n"
    "    ctx.fillText('UPCOMING', rx+8*s, h*0.42+11*s);\n"
    "    ctx.fillStyle = tClr(p,0.7); ctx.font = '500 '+(6*s)+'px Inter, sans-serif';\n"
    "    ctx.fillText('Hydrafacial — 60 min', rx+8*s, h*0.42+22*s);\n"
    "    ctx.fillStyle = tClr(p,0.38); ctx.font = '300 '+(5.5*s)+'px Inter, sans-serif';\n"
    "    ctx.fillText('Fri, May 9  ·  10:30 AM', rx+8*s, h*0.42+32*s);\n"
    '    ctx.beginPath(); ctx.arc(rx+108*s, h*0.42+20*s, 9*s, 0, Math.PI*2);\n'
    "    ctx.fillStyle = '#c4b09a'; ctx.fill();\n"
    "    ctx.fillStyle = 'rgba(255,255,255,0.85)'; ctx.font = '500 '+(5*s)+'px Inter, sans-serif';\n"
    "    ctx.textAlign = 'center'; ctx.fillText('AC', rx+108*s, h*0.42+23*s);\n"
    "    ctx.textAlign = 'left';\n"
    '\n'
    '    // Feature list\n'
    '    var features = [\n'
    "      {t:'Smart Scheduling', d:'Real-time availability'},\n"
    "      {t:'Service Catalog', d:'Treatments & pricing'},\n"
    "      {t:'Provider Profiles', d:'Book your preferred provider'},\n"
    "      {t:'Loyalty Rewards', d:'Points & member perks'},\n"
    "      {t:'Push Reminders', d:'Appointment alerts'}\n"
    '    ];\n'
    "    ctx.font = (7.5*s)+'px Inter, sans-serif';\n"
    '    for (var fi = 0; fi < features.length; fi++) {\n'
    '      var fy = h*0.57 + fi*19*s;\n'
    "      ctx.fillStyle = aClr(p,0.4); ctx.fillText('|', rx, fy+4*s);\n"
    "      ctx.fillStyle = tClr(p,0.55); ctx.font = '400 '+(7*s)+'px Inter, sans-serif';\n"
    '      ctx.fillText(features[fi].t, rx+14*s, fy+4*s);\n'
    "      ctx.fillStyle = tClr(p,0.2); ctx.font = '200 '+(6.5*s)+'px Inter, sans-serif';\n"
    '      ctx.fillText(features[fi].d, rx+105*s, fy+4*s);\n'
    "      ctx.font = (7.5*s)+'px Inter, sans-serif';\n"
    '    }\n'
    '\n'
    '    drawCorners(ctx, w, h, 14*s, aClr(p,0.15));\n'
    '  }\n'
)

# Replace lines 587-722 (0-indexed: 586-721 inclusive)
new_lines = lines[:586] + [new_fn] + lines[722:]

with open('D:/ProjectsHome/FisherSovereign/lancewfisher-v2/deploy/main.js', 'w', encoding='utf-8') as f:
    f.writelines(new_lines)

print('Done. New line count:', len(new_lines))
