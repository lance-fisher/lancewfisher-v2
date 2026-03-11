// Transform canvas renderers for light theme support
// Adds tClr/aClr helpers and replaces hardcoded colors with theme-aware versions
const fs = require('fs');
const filePath = 'D:/ProjectsHome/lancewfisher-v2/main.js';
let code = fs.readFileSync(filePath, 'utf8');

// Step 1: Add tClr and aClr helper functions after pal() closing
const palEnd = "      light: light\n    };\n  }";
if (!code.includes('function tClr(')) {
  const palIdx = code.indexOf(palEnd);
  if (palIdx >= 0) {
    const insertion = `

  // Theme-aware text color: swaps warm-white to dark text
  function tClr(p, a) {
    return p.light ? 'rgba(26,26,28,' + Math.min(1, a * 1.15).toFixed(3) + ')' : 'rgba(240,235,224,' + a + ')';
  }
  // Theme-aware accent color: boosts alpha on light backgrounds
  function aClr(p, a) {
    var boosted = p.light ? Math.min(1, a < 0.1 ? a * 2.5 : a * 1.5) : a;
    return 'rgba(' + p.r + ',' + p.g + ',' + p.b + ',' + (boosted < 0.001 ? a : +boosted.toFixed(4)) + ')';
  }`;
    code = code.substring(0, palIdx + palEnd.length) + insertion + code.substring(palIdx + palEnd.length);
    console.log('Added tClr and aClr helpers');
  }
}

// Step 2: Renderer definitions
const renderers = [
  ['drawBooking', 184, 156, 92, true],
  ['drawTrading', 58, 143, 212, true],
  ['drawProfitDesk', 180, 120, 80, true],
  ['drawRTS', 192, 160, 80, true],
  ['drawMessenger', 138, 106, 212, true],
  ['drawBookmarkBot', 64, 180, 140, true],
  ['drawHomeHub', 60, 140, 220, true],
  ['drawProjectHub', 201, 168, 76, true],
  ['drawNoCo', 201, 168, 76, false],
  ['drawAuton', 160, 120, 220, false],
  ['drawMarketDash', 0, 220, 160, false],
  ['drawFisher13', 140, 170, 210, false],
  ['drawPolymarket', 0, 200, 80, false],
  ['drawGalleon', 120, 150, 180, false],
  ['drawPlatformer', 106, 196, 74, false],
];

// Find function boundaries
function findFunc(code, funcName) {
  const pattern = '  function ' + funcName + '(canvas) {';
  const startIdx = code.indexOf(pattern);
  if (startIdx === -1) return null;
  let depth = 0, i = startIdx, foundFirst = false;
  while (i < code.length) {
    if (code[i] === '{') { depth++; foundFirst = true; }
    if (code[i] === '}') depth--;
    if (foundFirst && depth === 0) {
      return { start: startIdx, end: i + 1, body: code.substring(startIdx, i + 1) };
    }
    i++;
  }
  return null;
}

// Process each renderer using indexOf-based replacement (avoids $ issues)
let totalChanges = 0;
for (const [funcName, r, g, b, usesDrawBg] of renderers) {
  const func = findFunc(code, funcName);
  if (!func) { console.log('NOT FOUND: ' + funcName); continue; }

  let body = func.body;
  let changes = 0;

  function bReplaceAll(search, repl) {
    let idx = 0;
    while (true) {
      const pos = body.indexOf(search, idx);
      if (pos === -1) break;
      body = body.substring(0, pos) + repl + body.substring(pos + search.length);
      idx = pos + repl.length;
      changes++;
    }
  }

  // Replace rgba prefix with theme ternary, capturing alpha from the original
  // e.g. rgbaSwap("'rgba(0,200,128,", "0,140,90") turns
  //   'rgba(0,200,128,0.8)' into (p.light ? 'rgba(0,140,90,0.8)' : 'rgba(0,200,128,0.8)')
  function rgbaSwap(origPrefix, lightRGB) {
    let idx = 0;
    while (true) {
      const pos = body.indexOf(origPrefix, idx);
      if (pos === -1) break;
      // Find the closing )' after the alpha
      const closePos = body.indexOf(")'", pos + origPrefix.length);
      if (closePos === -1) { idx = pos + 1; continue; }
      const alpha = body.substring(pos + origPrefix.length, closePos);
      const fullOrig = body.substring(pos, closePos + 2); // includes the quotes
      const repl = "(p.light ? 'rgba(" + lightRGB + "," + alpha + ")' : 'rgba(" + origPrefix.substring(1).slice(0,-1).replace(/^rgba\(/, '') + alpha + ")')";
      // Actually simpler: reconstruct both sides
      const origRGB = origPrefix.substring("'rgba(".length); // e.g. "0,200,128,"
      const replStr = "(p.light ? 'rgba(" + lightRGB + "," + alpha + ")' : " + fullOrig + ")";
      body = body.substring(0, pos) + replStr + body.substring(closePos + 2);
      idx = pos + replStr.length;
      changes++;
    }
  }

  function bReplaceOnce(search, repl) {
    const pos = body.indexOf(search);
    if (pos === -1) return false;
    body = body.substring(0, pos) + repl + body.substring(pos + search.length);
    changes++;
    return true;
  }

  // Add var p = pal(r,g,b)
  if (!body.includes('var p = pal(')) {
    if (usesDrawBg) {
      bReplaceOnce(
        'drawBg(ctx, w, h, ' + r + ', ' + g + ', ' + b + ');',
        'drawBg(ctx, w, h, ' + r + ', ' + g + ', ' + b + ');\n    var p = pal(' + r + ', ' + g + ', ' + b + ');'
      );
    } else {
      bReplaceOnce(
        'var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;',
        'var o = setupCanvas(canvas), ctx = o.ctx, w = o.w, h = o.h, s = o.s;\n    var p = pal(' + r + ', ' + g + ', ' + b + ');'
      );
    }
  }

  // Replace text colors rgba(240,235,224,X) -> tClr(p,X)
  const textRe = /\'rgba\(240,235,224,([\d.]+)\)\'/g;
  const textMatches = [];
  let m;
  while ((m = textRe.exec(body)) !== null) {
    textMatches.push({ idx: m.index, len: m[0].length, alpha: m[1] });
  }
  for (let i = textMatches.length - 1; i >= 0; i--) {
    const tm = textMatches[i];
    body = body.substring(0, tm.idx) + 'tClr(p,' + tm.alpha + ')' + body.substring(tm.idx + tm.len);
    changes++;
  }

  // Replace accent colors rgba(R,G,B,X) -> aClr(p,X)
  const accentStr = "'rgba(" + r + "," + g + "," + b + ",";
  let si = 0;
  while (true) {
    const pos = body.indexOf(accentStr, si);
    if (pos === -1) break;
    const closePos = body.indexOf(")'", pos + accentStr.length);
    if (closePos === -1) { si = pos + 1; continue; }
    const alpha = body.substring(pos + accentStr.length, closePos);
    if (/^[\d.]+$/.test(alpha)) {
      const repl = 'aClr(p,' + alpha + ')';
      body = body.substring(0, pos) + repl + body.substring(closePos + 2);
      si = pos + repl.length;
      changes++;
    } else {
      si = pos + 1;
    }
  }

  // Replace drawCorners color
  const cornerStr = "drawCorners(ctx, w, h, 14*s, 'rgba(" + r + "," + g + "," + b + ",";
  const cPos = body.indexOf(cornerStr);
  if (cPos >= 0) {
    const cEnd = body.indexOf(")')", cPos);
    if (cEnd >= 0) {
      body = body.substring(0, cPos) + 'drawCorners(ctx, w, h, 14*s, p.cornerColor)' + body.substring(cEnd + 3);
      changes++;
    }
  }

  // Replace dark shadows rgba(0,0,0,X) where X >= 0.2
  const shadowRe = /\'rgba\(0,0,0,(0\.[2-9]\d*)\)\'/g;
  const sMatches = [];
  while ((m = shadowRe.exec(body)) !== null) {
    sMatches.push({ idx: m.index, len: m[0].length, alpha: m[1] });
  }
  for (let i = sMatches.length - 1; i >= 0; i--) {
    const sm = sMatches[i];
    const la = (parseFloat(sm.alpha) * 0.4).toFixed(2);
    const repl = "(p.light ? 'rgba(26,26,28," + la + ")' : 'rgba(0,0,0," + sm.alpha + ")')";
    body = body.substring(0, sm.idx) + repl + body.substring(sm.idx + sm.len);
    changes++;
  }

  // Custom backgrounds for non-drawBg renderers
  if (funcName === 'drawNoCo') {
    bReplaceOnce("bg.addColorStop(0, '#0c0a08'); bg.addColorStop(0.5, '#0e0b09');\n    bg.addColorStop(1, '#0a0907');",
      "if (p.light) { bg.addColorStop(0, '#e8e6e3'); bg.addColorStop(0.5, '#e5e3e0'); bg.addColorStop(1, '#e2e0dd'); }\n    else { bg.addColorStop(0, '#0c0a08'); bg.addColorStop(0.5, '#0e0b09'); bg.addColorStop(1, '#0a0907'); }");
    bReplaceOnce("ctx.fillStyle = 'rgba(201,168,76,0.012)'; ctx.fill();",
      "ctx.fillStyle = p.light ? 'rgba(201,168,76,0.03)' : 'rgba(201,168,76,0.012)'; ctx.fill();");
  }
  if (funcName === 'drawAuton') {
    bReplaceOnce("bg.addColorStop(0, '#090710'); bg.addColorStop(0.5, '#0b0912');\n    bg.addColorStop(1, '#07060c');",
      "if (p.light) { bg.addColorStop(0, '#e8e6ec'); bg.addColorStop(0.5, '#e5e3ea'); bg.addColorStop(1, '#e2e0e8'); }\n    else { bg.addColorStop(0, '#090710'); bg.addColorStop(0.5, '#0b0912'); bg.addColorStop(1, '#07060c'); }");
  }
  if (funcName === 'drawMarketDash') {
    bReplaceOnce("bg.addColorStop(0, '#080a0c'); bg.addColorStop(0.5, '#060a0e');\n    bg.addColorStop(1, '#04080c');",
      "if (p.light) { bg.addColorStop(0, '#e6e8ea'); bg.addColorStop(0.5, '#e3e7ea'); bg.addColorStop(1, '#e0e5e9'); }\n    else { bg.addColorStop(0, '#080a0c'); bg.addColorStop(0.5, '#060a0e'); bg.addColorStop(1, '#04080c'); }");
    bReplaceOnce("ctx.fillStyle = 'rgba(0,255,180,0.003)';",
      "ctx.fillStyle = p.light ? 'rgba(0,180,120,0.008)' : 'rgba(0,255,180,0.003)';");
    rgbaSwap("'rgba(0,220,150,", "0,160,110");
    bReplaceOnce("ctx.fillStyle = '#00dc96'; ctx.fill();",
      "ctx.fillStyle = p.light ? '#00a070' : '#00dc96'; ctx.fill();");
  }
  if (funcName === 'drawFisher13') {
    bReplaceOnce("sky.addColorStop(0, '#050810'); sky.addColorStop(0.35, '#0a1020');\n    sky.addColorStop(0.7, '#0c1428'); sky.addColorStop(1, '#060a14');",
      "if (p.light) { sky.addColorStop(0, '#dfe5ed'); sky.addColorStop(0.35, '#dae2ec'); sky.addColorStop(0.7, '#d5dde9'); sky.addColorStop(1, '#dfe5ed'); }\n    else { sky.addColorStop(0, '#050810'); sky.addColorStop(0.35, '#0a1020'); sky.addColorStop(0.7, '#0c1428'); sky.addColorStop(1, '#060a14'); }");
    bReplaceOnce("ocean.addColorStop(0, 'rgba(15,30,60,0.0)');", "ocean.addColorStop(0, p.light ? 'rgba(140,170,210,0.0)' : 'rgba(15,30,60,0.0)');");
    bReplaceOnce("ocean.addColorStop(0.3, 'rgba(15,30,60,0.2)');", "ocean.addColorStop(0.3, p.light ? 'rgba(140,170,210,0.08)' : 'rgba(15,30,60,0.2)');");
    bReplaceOnce("ocean.addColorStop(1, 'rgba(8,16,32,0.6)');", "ocean.addColorStop(1, p.light ? 'rgba(140,170,210,0.15)' : 'rgba(8,16,32,0.6)');");
    bReplaceOnce("vigTop.addColorStop(0, 'rgba(5,8,16,1)'); vigTop.addColorStop(1, 'rgba(5,8,16,0)');",
      "vigTop.addColorStop(0, p.light ? 'rgba(223,229,237,1)' : 'rgba(5,8,16,1)'); vigTop.addColorStop(1, p.light ? 'rgba(223,229,237,0)' : 'rgba(5,8,16,0)');");
    bReplaceOnce("vigBot.addColorStop(0, 'rgba(5,8,16,0)'); vigBot.addColorStop(1, 'rgba(5,8,16,0.9)');",
      "vigBot.addColorStop(0, p.light ? 'rgba(223,229,237,0)' : 'rgba(5,8,16,0)'); vigBot.addColorStop(1, p.light ? 'rgba(223,229,237,0.9)' : 'rgba(5,8,16,0.9)');");
    bReplaceOnce("ctx.strokeStyle = 'rgba(140,170,210,0.04)'; ctx.lineWidth = 0.6*s;",
      "ctx.strokeStyle = p.light ? 'rgba(100,130,170,0.08)' : 'rgba(140,170,210,0.04)'; ctx.lineWidth = 0.6*s;");
    bReplaceOnce("ctx.fillStyle = 'rgba(200,215,240,'+sa+')';",
      "ctx.fillStyle = p.light ? 'rgba(100,115,140,'+sa+')' : 'rgba(200,215,240,'+sa+')';");
    bReplaceOnce("ctx.strokeStyle = '#8caad2'; ctx.lineWidth = 1*s;",
      "ctx.strokeStyle = p.light ? '#6080a0' : '#8caad2'; ctx.lineWidth = 1*s;");
    bReplaceOnce("ctx.fillStyle = '#8caad2'; ctx.font = '600 '+(5*s)+'px Inter, sans-serif';",
      "ctx.fillStyle = p.light ? '#6080a0' : '#8caad2'; ctx.font = '600 '+(5*s)+'px Inter, sans-serif';");
  }
  if (funcName === 'drawPolymarket') {
    bReplaceOnce("ctx.fillStyle = '#020204'; ctx.fillRect(0, 0, w, h);",
      "ctx.fillStyle = p.light ? '#e8e9eb' : '#020204'; ctx.fillRect(0, 0, w, h);");
    bReplaceOnce("ctx.fillStyle = 'rgba(0,255,100,0.006)'; ctx.fillRect(0, sl, w, 1.5*s);",
      "ctx.fillStyle = p.light ? 'rgba(0,140,60,0.012)' : 'rgba(0,255,100,0.006)'; ctx.fillRect(0, sl, w, 1.5*s);");
    bReplaceOnce("vig.addColorStop(1, 'rgba(0,0,0,0.3)');",
      "vig.addColorStop(1, p.light ? 'rgba(26,26,28,0.08)' : 'rgba(0,0,0,0.3)');");
    rgbaSwap("'rgba(0,200,128,", "0,140,90");
    rgbaSwap("'rgba(0,170,100,", "0,120,70");
    rgbaSwap("'rgba(100,100,100,", "26,26,28");
    rgbaSwap("'rgba(80,80,80,", "26,26,28");
    rgbaSwap("'rgba(140,140,140,", "60,60,60");
    rgbaSwap("'rgba(0,200,80,", "0,140,60");
  }
  if (funcName === 'drawGalleon') {
    bReplaceOnce("sky.addColorStop(0, '#060a10'); sky.addColorStop(0.3, '#0e141c'); sky.addColorStop(0.7, '#101820'); sky.addColorStop(1, '#060a0e');",
      "if (p.light) { sky.addColorStop(0, '#dde2e8'); sky.addColorStop(0.3, '#d8dee6'); sky.addColorStop(0.7, '#d5dce5'); sky.addColorStop(1, '#dde2e8'); }\n    else { sky.addColorStop(0, '#060a10'); sky.addColorStop(0.3, '#0e141c'); sky.addColorStop(0.7, '#101820'); sky.addColorStop(1, '#060a0e'); }");
    bReplaceOnce("ocean.addColorStop(0, 'rgba(10,30,50,0.3)'); ocean.addColorStop(1, 'rgba(5,15,25,0.8)');",
      "ocean.addColorStop(0, p.light ? 'rgba(120,150,180,0.08)' : 'rgba(10,30,50,0.3)'); ocean.addColorStop(1, p.light ? 'rgba(120,150,180,0.15)' : 'rgba(5,15,25,0.8)');");
    bReplaceOnce("fade.addColorStop(0, 'rgba(6,10,14,0)'); fade.addColorStop(1, 'rgba(6,10,14,1)');",
      "fade.addColorStop(0, p.light ? 'rgba(221,226,232,0)' : 'rgba(6,10,14,0)'); fade.addColorStop(1, p.light ? 'rgba(221,226,232,1)' : 'rgba(6,10,14,1)');");
    bReplaceOnce("cg.addColorStop(0, 'rgba(30,40,55,0.15)');",
      "cg.addColorStop(0, p.light ? 'rgba(120,150,180,0.06)' : 'rgba(30,40,55,0.15)');");
    rgbaSwap("'rgba(64,200,160,", "40,120,100");
    bReplaceOnce("ctx.strokeStyle = 'rgba(80,120,160,0.06)'; ctx.lineWidth = 0.5*s;",
      "ctx.strokeStyle = p.light ? 'rgba(80,120,160,0.1)' : 'rgba(80,120,160,0.06)'; ctx.lineWidth = 0.5*s;");
  }
  if (funcName === 'drawPlatformer') {
    bReplaceOnce("sky.addColorStop(0, '#0c1628'); sky.addColorStop(0.3, '#1a2d4a'); sky.addColorStop(0.6, '#243a58'); sky.addColorStop(1, '#162a1e');",
      "if (p.light) { sky.addColorStop(0, '#b8d4f0'); sky.addColorStop(0.3, '#a0c8e8'); sky.addColorStop(0.6, '#90bbe0'); sky.addColorStop(1, '#a8d8b8'); }\n    else { sky.addColorStop(0, '#0c1628'); sky.addColorStop(0.3, '#1a2d4a'); sky.addColorStop(0.6, '#243a58'); sky.addColorStop(1, '#162a1e'); }");
    bReplaceOnce("ctx.fillStyle = 'rgba(20,40,30,0.4)'; ctx.fill();",
      "ctx.fillStyle = p.light ? 'rgba(60,100,70,0.15)' : 'rgba(20,40,30,0.4)'; ctx.fill();");
    bReplaceOnce("hudGrad.addColorStop(0, 'rgba(0,0,0,0.7)'); hudGrad.addColorStop(1, 'rgba(0,0,0,0.3)');",
      "hudGrad.addColorStop(0, p.light ? 'rgba(26,26,28,0.5)' : 'rgba(0,0,0,0.7)'); hudGrad.addColorStop(1, p.light ? 'rgba(26,26,28,0.15)' : 'rgba(0,0,0,0.3)');");
  }

  // RTS HUD overlays
  if (funcName === 'drawRTS') {
    bReplaceOnce("ctx.fillStyle = 'rgba(0,0,0,0.65)'; ctx.fillRect(0, 0, w, 22*s);",
      "ctx.fillStyle = p.light ? 'rgba(26,26,28,0.06)' : 'rgba(0,0,0,0.65)'; ctx.fillRect(0, 0, w, 22*s);");
    bReplaceOnce("ctx.fillStyle = 'rgba(0,0,0,0.55)';\n    ctx.fillRect(mmx, mmy, mmw, mmh);",
      "ctx.fillStyle = p.light ? 'rgba(26,26,28,0.06)' : 'rgba(0,0,0,0.55)';\n    ctx.fillRect(mmx, mmy, mmw, mmh);");
    bReplaceOnce("ctx.fillStyle = 'rgba(0,0,0,0.6)'; ctx.fillRect(0, h-26*s, w*0.65, 26*s);",
      "ctx.fillStyle = p.light ? 'rgba(26,26,28,0.06)' : 'rgba(0,0,0,0.6)'; ctx.fillRect(0, h-26*s, w*0.65, 26*s);");
  }
  if (funcName === 'drawHomeHub') {
    bReplaceOnce("ctx.fillStyle = 'rgba(0,0,0,0.3)'; ctx.fillRect(fx+2*s, fy+2*s, 40*s, 9*s);",
      "ctx.fillStyle = p.light ? 'rgba(26,26,28,0.06)' : 'rgba(0,0,0,0.3)'; ctx.fillRect(fx+2*s, fy+2*s, 40*s, 9*s);");
  }
  if (funcName === 'drawProjectHub') {
    bReplaceOnce("'rgba(240,235,224,0.008)'", "(p.light ? 'rgba(26,26,28,0.015)' : 'rgba(240,235,224,0.008)')");
  }
  if (funcName === 'drawBookmarkBot') {
    bReplaceOnce("ctx.fillStyle = 'rgba(240,235,224,0.08)'; ctx.font = 'bold '+(40*s)+'px Inter, sans-serif';",
      "ctx.fillStyle = p.light ? 'rgba(26,26,28,0.04)' : 'rgba(240,235,224,0.08)'; ctx.font = 'bold '+(40*s)+'px Inter, sans-serif';");
  }

  if (changes > 0) {
    // Safe replacement using indexOf (avoids $ interpretation)
    const startIdx = code.indexOf(func.body);
    if (startIdx >= 0) {
      code = code.substring(0, startIdx) + body + code.substring(startIdx + func.body.length);
      totalChanges++;
      console.log('Modified: ' + funcName + ' (' + changes + ' changes)');
    } else {
      console.log('ERROR replacing: ' + funcName);
    }
  } else {
    console.log('No changes: ' + funcName);
  }
}

fs.writeFileSync(filePath, code);
console.log('\nDone! Modified ' + totalChanges + ' renderers.');
