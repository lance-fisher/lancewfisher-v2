export function clamp(value, min, max) {
  return Math.max(min, Math.min(max, value));
}

export function distance(aX, aY, bX, bY) {
  return Math.hypot(bX - aX, bY - aY);
}

export function moveToward(currentX, currentY, targetX, targetY, speed, dt) {
  const deltaX = targetX - currentX;
  const deltaY = targetY - currentY;
  const length = Math.hypot(deltaX, deltaY);

  if (length === 0 || length <= speed * dt) {
    return { x: targetX, y: targetY, arrived: true };
  }

  return {
    x: currentX + (deltaX / length) * speed * dt,
    y: currentY + (deltaY / length) * speed * dt,
    arrived: false,
  };
}

export function rectContainsPoint(rect, x, y) {
  return x >= rect.x && x <= rect.x + rect.w && y >= rect.y && y <= rect.y + rect.h;
}

export function formatResourceValue(value) {
  return Number.isInteger(value) ? String(value) : value.toFixed(1);
}
