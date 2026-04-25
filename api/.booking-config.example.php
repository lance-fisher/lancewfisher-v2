<?php
/**
 * Copy this file to .booking-config.php and replace the placeholder password.
 * Hostinger SMTP reference:
 * - Host: smtp.hostinger.com
 * - Port: 465 (SSL) or 587 (TLS/STARTTLS)
 * - Username: full mailbox address
 * - Password: mailbox password
 */

return [
    'recipient' => 'lance@lancewfisher.com',
    'site_name' => 'lancewfisher.com',
    'from_name' => 'Lance Fisher',
    'from_address' => 'lance@lancewfisher.com',
    'sms_to' => '',
    'smtp' => [
        'enabled' => true,
        'host' => 'smtp.hostinger.com',
        'port' => 465,
        'secure' => 'ssl',
        'username' => 'lance@lancewfisher.com',
        'password' => 'replace-with-your-hostinger-mailbox-password',
        'timeout' => 20,
        'ehlo_domain' => 'lancewfisher.com',
    ],
];
