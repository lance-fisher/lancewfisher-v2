"""
Generate Jump Quest thumbnail for lancewfisher-v2 portfolio.
800x600, dark blue/purple gaming aesthetic.
"""

from PIL import Image, ImageDraw, ImageFont
import math
import os
import random

WIDTH, HEIGHT = 800, 600
FONT_DIR = "C:/Windows/Fonts"

def font(name, size):
    path = os.path.join(FONT_DIR, name)
    return ImageFont.truetype(path, size)

arial_bold = lambda s: font("arialbd.ttf", s)
arial_reg = lambda s: font("arial.ttf", s)
impact_fn = lambda s: font("impact.ttf", s)
segoe_bold = lambda s: font("segoeuib.ttf", s)
consolas_bold = lambda s: font("consolab.ttf", s)

img = Image.new("RGBA", (WIDTH, HEIGHT), (0, 0, 0, 255))
draw = ImageDraw.Draw(img)

c1 = (13, 13, 43)
c2 = (26, 26, 78)
c3 = (45, 26, 94)

for y in range(HEIGHT):
    for x in range(WIDTH):
        t = (x / WIDTH * 0.4 + y / HEIGHT * 0.6)
        if t < 0.5:
            lt = t / 0.5
            r = int(c1[0] + (c2[0] - c1[0]) * lt)
            g = int(c1[1] + (c2[1] - c1[1]) * lt)
            b = int(c1[2] + (c2[2] - c1[2]) * lt)
        else:
            lt = (t - 0.5) / 0.5
            r = int(c2[0] + (c3[0] - c2[0]) * lt)
            g = int(c2[1] + (c3[1] - c2[1]) * lt)
            b = int(c2[2] + (c3[2] - c2[2]) * lt)
        img.putpixel((x, y), (r, g, b, 255))

draw = ImageDraw.Draw(img)

random.seed(42)
for _ in range(60):
    sx = random.randint(0, WIDTH - 1)
    sy = random.randint(0, HEIGHT - 1)
    brightness = random.randint(40, 120)
    size = random.choice([1, 1, 1, 2])
    draw.ellipse([sx, sy, sx + size, sy + size],
                 fill=(brightness, brightness, brightness + 40, brightness))

glow_layer = Image.new("RGBA", (WIDTH, HEIGHT), (0, 0, 0, 0))
glow_draw = ImageDraw.Draw(glow_layer)
cx, cy = WIDTH // 2, HEIGHT // 2 - 30
for radius in range(200, 0, -2):
    alpha = int(15 * (1 - radius / 200))
    glow_draw.ellipse(
        [cx - radius, cy - radius, cx + radius, cy + radius],
        fill=(100, 80, 180, alpha)
    )
img = Image.alpha_composite(img, glow_layer)
draw = ImageDraw.Draw(img)

def draw_centered(y, text, fnt, fill):
    bbox = draw.textbbox((0, 0), text, font=fnt)
    tw = bbox[2] - bbox[0]
    x = (WIDTH - tw) // 2
    draw.text((x, y), text, font=fnt, fill=fill)
    return bbox[3] - bbox[1]

def draw_spaced_centered(y, text, fnt, fill, spacing=4):
    total_w = 0
    for i, ch in enumerate(text):
        bbox = draw.textbbox((0, 0), ch, font=fnt)
        total_w += bbox[2] - bbox[0]
        if i < len(text) - 1:
            total_w += spacing
    x = (WIDTH - total_w) // 2
    ch_h = 0
    for i, ch in enumerate(text):
        bbox = draw.textbbox((0, 0), ch, font=fnt)
        cw = bbox[2] - bbox[0]
        ch_h = bbox[3] - bbox[1]
        draw.text((x, y), ch, font=fnt, fill=fill)
        x += cw + spacing
    return ch_h

ARROW = chr(8594)

y_pos = 35
draw_spaced_centered(y_pos, "WELCOME TO", arial_reg(18), (170, 170, 170), spacing=6)

y_pos = 62
draw_centered(y_pos, "FISHER GAMING" + chr(39) + "S", impact_fn(42), (255, 153, 51))

line_y = 112
line_half = 140
draw.line([(WIDTH // 2 - line_half, line_y), (WIDTH // 2 + line_half, line_y)],
          fill=(255, 153, 51, 120), width=2)

y_pos = 125
fnt_main = impact_fn(80)
bbox = draw.textbbox((0, 0), "JUMP QUEST!", font=fnt_main)
tw = bbox[2] - bbox[0]
tx = (WIDTH - tw) // 2
draw.text((tx + 3, y_pos + 3), "JUMP QUEST!", font=fnt_main, fill=(0, 0, 0, 180))
draw.text((tx, y_pos), "JUMP QUEST!", font=fnt_main, fill=(255, 230, 100))
highlight_layer = Image.new("RGBA", (WIDTH, HEIGHT), (0, 0, 0, 0))
h_draw = ImageDraw.Draw(highlight_layer)
h_draw.text((tx, y_pos), "JUMP QUEST!", font=fnt_main, fill=(255, 255, 220, 80))
img = Image.alpha_composite(img, highlight_layer)
draw = ImageDraw.Draw(img)

line_y2 = 215
line_half2 = 180
draw.line([(WIDTH // 2 - line_half2, line_y2), (WIDTH // 2 + line_half2, line_y2)],
          fill=(255, 230, 100, 80), width=1)

box_x = 160
box_y = 230
box_w = 480
box_h = 230
box_r = 16

box_layer = Image.new("RGBA", (WIDTH, HEIGHT), (0, 0, 0, 0))
box_draw = ImageDraw.Draw(box_layer)
box_draw.rounded_rectangle(
    [box_x, box_y, box_x + box_w, box_y + box_h],
    radius=box_r,
    fill=(255, 255, 255, 20),
    outline=(255, 255, 255, 30),
    width=1
)
img = Image.alpha_composite(img, box_layer)
draw = ImageDraw.Draw(img)

htp_y = box_y + 14
draw_centered(htp_y, "HOW TO PLAY", segoe_bold(22), (255, 204, 50))

htp_line_y = htp_y + 32
draw.line([(box_x + 40, htp_line_y), (box_x + box_w - 40, htp_line_y)],
          fill=(255, 204, 50, 60), width=1)

controls = [
    ("W A S D", "Move around", (80, 160, 255)),
    ("Space Bar", "Jump!", (80, 220, 100)),
    ("Shift", "Run fast!", (255, 165, 60)),
    ("Escape", "Pause", (160, 160, 160)),
]

ctrl_y = htp_line_y + 12
ctrl_font_key = consolas_bold(18)
ctrl_font_desc = arial_reg(17)

for key_text, desc_text, key_color in controls:
    full_desc = " " + ARROW + "  " + desc_text
    key_bbox = draw.textbbox((0, 0), key_text, font=ctrl_font_key)
    key_w = key_bbox[2] - key_bbox[0]
    desc_bbox = draw.textbbox((0, 0), full_desc, font=ctrl_font_desc)
    desc_w = desc_bbox[2] - desc_bbox[0]
    total_w = key_w + 8 + desc_w
    start_x = (WIDTH - total_w) // 2

    pill_pad = 6
    pill_layer = Image.new("RGBA", (WIDTH, HEIGHT), (0, 0, 0, 0))
    pill_draw = ImageDraw.Draw(pill_layer)
    pill_draw.rounded_rectangle(
        [start_x - pill_pad, ctrl_y - 2,
         start_x + key_w + pill_pad, ctrl_y + (key_bbox[3] - key_bbox[1]) + 4],
        radius=6,
        fill=(key_color[0], key_color[1], key_color[2], 35)
    )
    img = Image.alpha_composite(img, pill_layer)
    draw = ImageDraw.Draw(img)

    draw.text((start_x, ctrl_y), key_text, font=ctrl_font_key, fill=key_color)
    draw.text((start_x + key_w + 8, ctrl_y + 1), full_desc,
              font=ctrl_font_desc, fill=(220, 220, 220))
    ctrl_y += 38

tag_y = box_y + box_h + 18
draw_centered(tag_y, "Collect coins!  Jump across platforms!  Reach the finish to win!",
              arial_reg(16), (255, 204, 80))

start_y = HEIGHT - 60
start_font = segoe_bold(24)
start_text = "Press any key or click to start!"

pulse_layer = Image.new("RGBA", (WIDTH, HEIGHT), (0, 0, 0, 0))
p_draw = ImageDraw.Draw(pulse_layer)
s_bbox = p_draw.textbbox((0, 0), start_text, font=start_font)
s_tw = s_bbox[2] - s_bbox[0]
s_tx = (WIDTH - s_tw) // 2
for r in range(30, 0, -3):
    alpha = int(8 * (1 - r / 30))
    p_draw.rounded_rectangle(
        [s_tx - r, start_y - r // 2, s_tx + s_tw + r, start_y + 30 + r // 2],
        radius=r,
        fill=(255, 255, 255, alpha)
    )
img = Image.alpha_composite(img, pulse_layer)
draw = ImageDraw.Draw(img)

draw.text((s_tx, start_y), start_text, font=start_font, fill=(255, 255, 255))

corner_layer = Image.new("RGBA", (WIDTH, HEIGHT), (0, 0, 0, 0))
c_draw = ImageDraw.Draw(corner_layer)
c_draw.polygon([(0, 0), (60, 0), (0, 60)], fill=(255, 153, 51, 25))
c_draw.polygon([(0, 0), (30, 0), (0, 30)], fill=(255, 153, 51, 40))
c_draw.polygon([(WIDTH, 0), (WIDTH - 60, 0), (WIDTH, 60)], fill=(255, 153, 51, 25))
c_draw.polygon([(WIDTH, 0), (WIDTH - 30, 0), (WIDTH, 30)], fill=(255, 153, 51, 40))
c_draw.polygon([(0, HEIGHT), (60, HEIGHT), (0, HEIGHT - 60)], fill=(100, 80, 180, 25))
c_draw.polygon([(WIDTH, HEIGHT), (WIDTH - 60, HEIGHT), (WIDTH, HEIGHT - 60)],
               fill=(100, 80, 180, 25))
img = Image.alpha_composite(img, corner_layer)

output_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), "thumb-jumpquest.jpg")
img_rgb = img.convert("RGB")
img_rgb.save(output_path, "JPEG", quality=90)
print(f"Saved thumbnail to: {output_path}")
print(f"Size: {img_rgb.size}")
