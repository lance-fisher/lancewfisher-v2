<?php
/**
 * Booking Form Handler — lancewfisher.com
 * Self-hosted form processor. No third-party services.
 *
 * Accepts POST with: name, email, phone (optional), topic, message
 * Sends formatted HTML email to site owner.
 * Returns JSON for AJAX consumption.
 */

// ─── Configuration ───────────────────────────────────────────────
define('RECIPIENT',    'lance@lancewfisher.com');
define('SITE_NAME',    'lancewfisher.com');
define('FROM_ADDRESS', 'noreply@lancewfisher.com');
define('RATE_WINDOW',  300);   // 5 minutes
define('RATE_LIMIT',   3);     // max submissions per window

// ─── Headers ─────────────────────────────────────────────────────
header('Content-Type: application/json; charset=utf-8');
header('X-Content-Type-Options: nosniff');

// Only allow POST
if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    http_response_code(405);
    echo json_encode(['ok' => false, 'error' => 'Method not allowed']);
    exit;
}

// ─── Honeypot check (spam bots fill hidden fields) ───────────────
if (!empty($_POST['_honey']) || !empty($_POST['website'])) {
    // Silently accept but don't send — fools the bot
    echo json_encode(['ok' => true]);
    exit;
}

// ─── Rate limiting (file-based, no database needed) ──────────────
$ratefile = sys_get_temp_dir() . '/lwf_booking_' . md5($_SERVER['REMOTE_ADDR'] ?? 'local') . '.json';
$now = time();
$submissions = [];

if (file_exists($ratefile)) {
    $data = json_decode(file_get_contents($ratefile), true);
    if (is_array($data)) {
        // Keep only entries within the window
        $submissions = array_filter($data, function($ts) use ($now) {
            return ($now - $ts) < RATE_WINDOW;
        });
    }
}

if (count($submissions) >= RATE_LIMIT) {
    http_response_code(429);
    echo json_encode(['ok' => false, 'error' => 'Too many requests. Please try again in a few minutes.']);
    exit;
}

// ─── Input sanitization ──────────────────────────────────────────
function clean($val) {
    return htmlspecialchars(strip_tags(trim($val ?? '')), ENT_QUOTES, 'UTF-8');
}

$name    = clean($_POST['name']    ?? '');
$email   = clean($_POST['email']   ?? '');
$phone   = clean($_POST['phone']   ?? '');
$topic   = clean($_POST['topic']   ?? '');
$message = clean($_POST['message'] ?? '');

// ─── Validation ──────────────────────────────────────────────────
$errors = [];
if ($name === '')    $errors[] = 'Name is required.';
if ($email === '')   $errors[] = 'Email is required.';
if (!filter_var($email, FILTER_VALIDATE_EMAIL)) $errors[] = 'Invalid email address.';
if ($topic === '')   $errors[] = 'Please select a topic.';
if ($message === '') $errors[] = 'Message is required.';

if (!empty($errors)) {
    http_response_code(422);
    echo json_encode(['ok' => false, 'errors' => $errors]);
    exit;
}

// ─── Build email ─────────────────────────────────────────────────
$date    = date('F j, Y \a\t g:i A T');
$ip      = $_SERVER['REMOTE_ADDR'] ?? 'unknown';
$ua      = $_SERVER['HTTP_USER_AGENT'] ?? 'unknown';
$referer = $_SERVER['HTTP_REFERER'] ?? 'direct';

$subject = "New Booking Request: $topic — $name";

// Plain-text version
$textBody = <<<TEXT
NEW BOOKING REQUEST — $date
━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Name:    $name
Email:   $email
Phone:   $phone
Topic:   $topic

Message:
$message

━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Source:  $referer
IP:      $ip
TEXT;

// HTML version
$htmlBody = <<<HTML
<!DOCTYPE html>
<html>
<head><meta charset="utf-8"></head>
<body style="margin:0;padding:0;background:#0a0a0b;font-family:Arial,Helvetica,sans-serif;">
<table width="100%" cellpadding="0" cellspacing="0" style="background:#0a0a0b;padding:40px 20px;">
<tr><td align="center">
<table width="560" cellpadding="0" cellspacing="0" style="background:#111113;border:1px solid rgba(201,168,76,0.15);border-radius:4px;">
  <tr><td style="padding:32px 32px 24px;border-bottom:1px solid rgba(201,168,76,0.1);">
    <div style="font-size:11px;text-transform:uppercase;letter-spacing:3px;color:#c9a84c;margin-bottom:8px;">Booking Request</div>
    <div style="font-size:22px;color:#f0ebe0;font-weight:600;">$name</div>
    <div style="font-size:13px;color:rgba(240,235,224,0.4);margin-top:4px;">$date</div>
  </td></tr>
  <tr><td style="padding:24px 32px;">
    <table width="100%" cellpadding="0" cellspacing="0">
      <tr>
        <td style="padding:8px 0;color:rgba(240,235,224,0.4);font-size:12px;text-transform:uppercase;letter-spacing:1px;width:80px;vertical-align:top;">Email</td>
        <td style="padding:8px 0;color:#f0ebe0;font-size:14px;"><a href="mailto:$email" style="color:#c9a84c;text-decoration:none;">$email</a></td>
      </tr>
      <tr>
        <td style="padding:8px 0;color:rgba(240,235,224,0.4);font-size:12px;text-transform:uppercase;letter-spacing:1px;width:80px;vertical-align:top;">Phone</td>
        <td style="padding:8px 0;color:#f0ebe0;font-size:14px;">$phone</td>
      </tr>
      <tr>
        <td style="padding:8px 0;color:rgba(240,235,224,0.4);font-size:12px;text-transform:uppercase;letter-spacing:1px;width:80px;vertical-align:top;">Topic</td>
        <td style="padding:8px 0;color:#c9a84c;font-size:14px;font-weight:600;">$topic</td>
      </tr>
    </table>
  </td></tr>
  <tr><td style="padding:0 32px 24px;">
    <div style="background:rgba(240,235,224,0.03);border:1px solid rgba(201,168,76,0.08);border-radius:4px;padding:20px;">
      <div style="font-size:11px;text-transform:uppercase;letter-spacing:1px;color:rgba(240,235,224,0.3);margin-bottom:8px;">Message</div>
      <div style="color:#f0ebe0;font-size:14px;line-height:1.7;white-space:pre-wrap;">$message</div>
    </div>
  </td></tr>
  <tr><td style="padding:16px 32px 24px;border-top:1px solid rgba(201,168,76,0.06);">
    <div style="font-size:11px;color:rgba(240,235,224,0.2);">Source: $referer</div>
  </td></tr>
</table>
</td></tr>
</table>
</body>
</html>
HTML;

// ─── Construct email with multipart MIME ─────────────────────────
$boundary = md5(uniqid(time()));

$headers  = "From: " . SITE_NAME . " <" . FROM_ADDRESS . ">\r\n";
$headers .= "Reply-To: $name <$email>\r\n";
$headers .= "MIME-Version: 1.0\r\n";
$headers .= "Content-Type: multipart/alternative; boundary=\"$boundary\"\r\n";
$headers .= "X-Mailer: lancewfisher-booking/1.0\r\n";

$body  = "--$boundary\r\n";
$body .= "Content-Type: text/plain; charset=UTF-8\r\n\r\n";
$body .= $textBody . "\r\n\r\n";
$body .= "--$boundary\r\n";
$body .= "Content-Type: text/html; charset=UTF-8\r\n\r\n";
$body .= $htmlBody . "\r\n\r\n";
$body .= "--$boundary--";

// ─── Send ────────────────────────────────────────────────────────
$sent = @mail(RECIPIENT, $subject, $body, $headers);

if ($sent) {
    // Record submission for rate limiting
    $submissions[] = $now;
    file_put_contents($ratefile, json_encode(array_values($submissions)));

    // Auto-reply to sender (optional confirmation)
    $replySubject = "Received: Your message to Lance Fisher";
    $replyHeaders  = "From: Lance Fisher <" . FROM_ADDRESS . ">\r\n";
    $replyHeaders .= "Reply-To: " . RECIPIENT . "\r\n";
    $replyHeaders .= "Content-Type: text/plain; charset=UTF-8\r\n";
    $replyBody = "Hi $name,\n\nThank you for reaching out. I've received your message about \"$topic\" and will respond within 24 hours.\n\nBest,\nLance Fisher\nlancewfisher.com";
    @mail($email, $replySubject, $replyBody, $replyHeaders);

    echo json_encode(['ok' => true, 'message' => 'Your request has been sent.']);
} else {
    http_response_code(500);
    echo json_encode(['ok' => false, 'error' => 'Failed to send. Please email lance@lancewfisher.com directly.']);
}
