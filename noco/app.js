// =============================================
// NoCo App Studio — Interactive Demo Engine
// =============================================

let selectedBusiness = null;
let currentTab = 'home';

// ─── INIT ───
document.addEventListener('DOMContentLoaded', () => {
  renderBusinessCards();
  setupFilters();
});

// ─── SMOOTH SCROLL ───
function scrollToSection(id) {
  document.getElementById(id)?.scrollIntoView({ behavior: 'smooth', block: 'start' });
}

// ─── BUSINESS CARD GRID ───
function renderBusinessCards(filter = 'all') {
  const container = document.getElementById('businessCards');
  const filtered = filter === 'all'
    ? BUSINESSES
    : BUSINESSES.filter(b => {
        const filters = filter.split(',');
        return filters.some(f => b.type.includes(f));
      });

  container.innerHTML = filtered.map(b => `
    <div class="business-card ${selectedBusiness?.id === b.id ? 'active' : ''}"
         onclick="selectBusiness('${b.id}')"
         data-type="${b.type}">
      <div class="business-card-logo" style="background:${b.colors.primary}">${b.logoInitial}</div>
      <div class="business-card-name">${b.name}</div>
      <div class="business-card-type">${b.type}</div>
      <div class="business-card-city">${b.city}, ${b.state}</div>
    </div>
  `).join('');
}

// ─── FILTER BUTTONS ───
function setupFilters() {
  document.querySelectorAll('.filter-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      document.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
      renderBusinessCards(btn.dataset.filter);
    });
  });
}

// ─── SELECT BUSINESS ───
function selectBusiness(id) {
  selectedBusiness = BUSINESSES.find(b => b.id === id);
  currentTab = 'home';

  document.getElementById('demoDisplay').style.display = 'block';
  renderBusinessCards(document.querySelector('.filter-btn.active')?.dataset.filter || 'all');
  renderPhoneScreen();
  renderInfoPanel();

  setTimeout(() => {
    document.getElementById('demoDisplay').scrollIntoView({ behavior: 'smooth', block: 'start' });
  }, 100);
}

// ─── FORMAT HELPERS ───
function formatPrice(cents, label) {
  if (cents < 100) return `$${cents}`;
  const dollars = Math.floor(cents / 100);
  return label ? `$${dollars} ${label}` : `$${dollars}`;
}

function formatDuration(minutes) {
  if (minutes === 0) return '';
  if (minutes < 60) return `${minutes} min`;
  const h = Math.floor(minutes / 60);
  const m = minutes % 60;
  return m > 0 ? `${h}h ${m}m` : `${h}h`;
}

function getGreeting() {
  const h = new Date().getHours();
  if (h < 12) return 'Good morning';
  if (h < 18) return 'Good afternoon';
  return 'Good evening';
}

function getServiceIcon(category) {
  const icons = {
    'Injectables': '&#128137;',
    'Laser': '&#9889;',
    'Skin': '&#10024;',
    'Body': '&#128170;',
    'IV Therapy': '&#128167;',
    'Wellness': '&#127807;',
    'Manicures': '&#128133;',
    'Pedicures': '&#129462;',
    'Nail Art': '&#127912;',
    'Waxing': '&#10024;',
    'Enhancements': '&#128142;',
    'Add-Ons': '&#10133;',
    'Hair Cuts': '&#9986;',
    'Color': '&#127912;',
    'Treatments': '&#10024;',
    'Spa Services': '&#128134;',
    'Blowouts': '&#128168;',
    'Haircuts': '&#9986;',
    'Beard': '&#129492;',
    'Premium': '&#11088;',
    'Grooming': '&#128134;',
    'Group Classes': '&#127947;',
    'Personal Training': '&#128170;',
    'Memberships': '&#127380;',
    'Recovery': '&#128164;',
    'Mat Classes': '&#129496;',
    'Reformer': '&#127947;',
    'Private Sessions': '&#127380;',
    'Workshops': '&#128218;',
    'Full Groom': '&#128054;',
    'Bath': '&#128704;',
    'Specialty': '&#10024;',
    'Adjustments': '&#129463;',
    'Therapy': '&#128588;',
    'Rehabilitation': '&#128170;',
  };
  for (const [key, icon] of Object.entries(icons)) {
    if (category?.includes(key)) return icon;
  }
  return '&#10024;';
}

function getTabIcon(tab) {
  const icons = { home: '&#127968;', services: '&#10024;', appointments: '&#128197;', wallet: '&#128179;', profile: '&#128100;' };
  return icons[tab] || '&#128196;';
}

// ─── RENDER PHONE SCREEN ───
function renderPhoneScreen() {
  const b = selectedBusiness;
  if (!b) return;

  const screen = document.getElementById('phoneScreen');
  const c = b.colors;
  const textColor = isLightColor(c.background) ? '#1e293b' : '#ffffff';
  const textSecondary = isLightColor(c.background) ? '#64748b' : 'rgba(255,255,255,0.6)';

  let content = '';

  if (currentTab === 'home') {
    content = renderHomeTab(b, c, textColor, textSecondary);
  } else if (currentTab === 'services') {
    content = renderServicesTab(b, c, textColor, textSecondary);
  } else if (currentTab === 'appointments') {
    content = renderAppointmentsTab(b, c, textColor, textSecondary);
  } else if (currentTab === 'wallet') {
    content = renderWalletTab(b, c, textColor, textSecondary);
  } else if (currentTab === 'profile') {
    content = renderProfileTab(b, c, textColor, textSecondary);
  }

  screen.innerHTML = `
    <div class="app-screen" style="background:${c.background}; color:${textColor};">
      ${content}
      ${renderTabBar(b, c, textColor, textSecondary)}
    </div>
  `;
}

function renderHomeTab(b, c, textColor, textSecondary) {
  const topServices = b.services.slice(0, 3);

  return `
    <!-- Header -->
    <div class="app-header" style="position:relative;">
      <div class="app-greeting" style="color:${textSecondary}">${getGreeting()},</div>
      <div class="app-user-name" style="color:${textColor}">Sarah</div>
      <div class="app-logo-circle" style="background:${c.primary}">${b.logoInitial}</div>
    </div>

    <!-- Quick Actions -->
    <div class="app-quick-actions">
      <div class="app-quick-action" onclick="switchTab('services')">
        <div class="app-quick-icon" style="background:${c.primary}20; color:${c.primary};">&#128197;</div>
        <div class="app-quick-label" style="color:${textColor}">Book Now</div>
      </div>
      <div class="app-quick-action">
        <div class="app-quick-icon" style="background:#22c55e20; color:#22c55e;">&#128179;</div>
        <div class="app-quick-label" style="color:${textColor}">$125.00</div>
      </div>
      <div class="app-quick-action">
        <div class="app-quick-icon" style="background:${c.secondary}20; color:${c.secondary};">&#128197;</div>
        <div class="app-quick-label" style="color:${textColor}">My Appts</div>
      </div>
    </div>

    <!-- Upcoming Appointment -->
    <div class="app-section">
      <div class="app-section-title" style="color:${textColor}">Upcoming Appointment</div>
      <div class="app-service-card" style="background:${c.surface};">
        <div class="app-service-icon" style="background:${c.primary}20;">${getServiceIcon(topServices[0]?.category)}</div>
        <div class="app-service-info">
          <div class="app-service-name" style="color:${textColor}">${topServices[0]?.name || 'Service'}</div>
          <div class="app-service-desc" style="color:${textSecondary}">with ${b.staff[0]?.name || 'Staff'}</div>
          <div style="font-size:11px; color:${c.primary}; font-weight:600; margin-top:4px;">
            &#128337; Tomorrow at 2:30 PM
          </div>
        </div>
        <div style="color:${textSecondary}; font-size:16px;">&#10095;</div>
      </div>
    </div>

    <!-- Promotions -->
    <div class="app-section">
      <div class="app-section-title" style="color:${textColor}">Special Offers</div>
      ${b.promotions.map(p => `
        <div class="app-promo-card" style="background:${c.surface};">
          <div class="app-promo-title" style="color:${textColor}">${p.title}</div>
          <div class="app-promo-body" style="color:${textSecondary}">${p.body}</div>
          <div class="app-promo-btn" style="background:${c.primary}">${p.cta}</div>
        </div>
      `).join('')}
    </div>

    <!-- Popular Services -->
    <div class="app-section">
      <div class="app-section-title" style="color:${textColor}">Popular Services</div>
      ${topServices.map(s => `
        <div class="app-service-card" style="background:${c.surface};" onclick="switchTab('services')">
          <div class="app-service-icon" style="background:${c.primary}15;">${getServiceIcon(s.category)}</div>
          <div class="app-service-info">
            <div class="app-service-name" style="color:${textColor}">${s.name}</div>
            <div class="app-service-meta">
              <div class="app-service-duration" style="color:${textSecondary}">${formatDuration(s.duration)}</div>
              <div class="app-service-price" style="color:${c.primary}">${formatPrice(s.price, s.priceLabel)}</div>
            </div>
          </div>
        </div>
      `).join('')}
    </div>

    <!-- Contact -->
    <div class="app-contact" style="background:${c.surface}; border:1px solid ${c.primary}20;">
      <div class="app-contact-name" style="color:${textColor}">${b.name}</div>
      <div class="app-contact-address" style="color:${textSecondary}">${b.address}</div>
      <div class="app-contact-phone" style="color:${c.primary}">${b.phone}</div>
    </div>

    <div style="height: 16px;"></div>
  `;
}

function renderServicesTab(b, c, textColor, textSecondary) {
  return `
    <!-- Header -->
    <div style="padding: 48px 16px 8px;">
      <div style="font-size:20px; font-weight:700; color:${textColor}">Services</div>
    </div>

    <!-- Category Chips -->
    <div class="app-nav-tabs">
      <div class="app-nav-tab" style="background:${c.primary}; color:#fff;">All</div>
      ${b.categories.map(cat => `
        <div class="app-nav-tab" style="background:${c.surface}; color:${textSecondary};">${cat}</div>
      `).join('')}
    </div>

    <!-- Services List -->
    <div class="app-section" style="padding-top: 12px;">
      ${b.services.map(s => `
        <div class="app-service-card" style="background:${c.surface};">
          <div class="app-service-icon" style="background:${c.primary}15;">${getServiceIcon(s.category)}</div>
          <div class="app-service-info">
            <div class="app-service-name" style="color:${textColor}">${s.name}</div>
            <div class="app-service-desc" style="color:${textSecondary}">${s.description}</div>
            <div class="app-service-meta">
              <div class="app-service-duration" style="color:${textSecondary}">&#128337; ${formatDuration(s.duration)}</div>
              <div class="app-service-price" style="color:${c.primary}">${formatPrice(s.price, s.priceLabel)}</div>
            </div>
          </div>
        </div>
      `).join('')}
    </div>

    <div style="height: 16px;"></div>
  `;
}

function renderAppointmentsTab(b, c, textColor, textSecondary) {
  const svc1 = b.services[0];
  const svc2 = b.services.length > 2 ? b.services[2] : b.services[0];
  const staff1 = b.staff[0];
  const staff2 = b.staff.length > 1 ? b.staff[1] : b.staff[0];

  return `
    <div style="padding: 48px 16px 8px;">
      <div style="font-size:20px; font-weight:700; color:${textColor}">Appointments</div>
    </div>
    <div class="app-nav-tabs">
      <div class="app-nav-tab" style="background:${c.primary}; color:#fff;">Upcoming</div>
      <div class="app-nav-tab" style="background:${c.surface}; color:${textSecondary};">Past</div>
    </div>
    <div class="app-section" style="padding-top:12px;">
      <div class="app-service-card" style="background:${c.surface};">
        <div class="app-service-icon" style="background:${c.primary}20;">${getServiceIcon(svc1?.category)}</div>
        <div class="app-service-info">
          <div class="app-service-name" style="color:${textColor}">${svc1?.name || 'Service'}</div>
          <div class="app-service-desc" style="color:${textSecondary}">with ${staff1?.name || 'Staff'}</div>
          <div style="font-size:11px; color:${c.primary}; font-weight:600; margin-top:4px;">Tomorrow at 2:30 PM</div>
          <div style="display:inline-block; margin-top:6px; font-size:10px; font-weight:600; padding:2px 8px; border-radius:10px; background:#22c55e20; color:#22c55e;">Confirmed</div>
        </div>
      </div>
      <div style="margin-top:20px; font-size:12px; font-weight:600; color:${textSecondary}; text-transform:uppercase; letter-spacing:0.5px;">Past</div>
      <div class="app-service-card" style="background:${c.surface}; opacity:0.7;">
        <div class="app-service-icon" style="background:${c.primary}10;">${getServiceIcon(svc2?.category)}</div>
        <div class="app-service-info">
          <div class="app-service-name" style="color:${textColor}">${svc2?.name || 'Service'}</div>
          <div class="app-service-desc" style="color:${textSecondary}">with ${staff2?.name || 'Staff'}</div>
          <div style="font-size:11px; color:${textSecondary}; margin-top:4px;">Jan 28, 2026</div>
          <div style="display:inline-block; margin-top:6px; font-size:10px; font-weight:600; padding:2px 8px; border-radius:10px; background:#3b82f620; color:#3b82f6;">Completed</div>
        </div>
      </div>
    </div>
    <div style="height: 16px;"></div>
  `;
}

function renderWalletTab(b, c, textColor, textSecondary) {
  const svc1 = b.services[0];
  return `
    <div style="padding: 48px 16px 8px;">
      <div style="font-size:20px; font-weight:700; color:${textColor}">Wallet</div>
    </div>
    <div class="app-section">
      <div style="background:linear-gradient(135deg, ${c.primary}, ${c.primaryDark || c.secondary}); border-radius:16px; padding:24px; color:#fff; margin-bottom:16px;">
        <div style="font-size:11px; opacity:0.8; text-transform:uppercase; letter-spacing:1px;">Available Balance</div>
        <div style="font-size:36px; font-weight:700; margin:8px 0;">$125.00</div>
        <div style="font-size:11px; opacity:0.7;">Loyalty points: 340 pts</div>
      </div>
      <div style="display:grid; grid-template-columns:repeat(3,1fr); gap:8px; margin-bottom:20px;">
        <div style="background:${c.surface}; border-radius:10px; padding:12px; text-align:center; cursor:pointer;">
          <div style="font-size:14px; font-weight:600; color:${textColor}">$50</div>
        </div>
        <div style="background:${c.primary}15; border:1px solid ${c.primary}40; border-radius:10px; padding:12px; text-align:center; cursor:pointer;">
          <div style="font-size:14px; font-weight:600; color:${c.primary}">$100</div>
        </div>
        <div style="background:${c.surface}; border-radius:10px; padding:12px; text-align:center; cursor:pointer;">
          <div style="font-size:14px; font-weight:600; color:${textColor}">$200</div>
        </div>
      </div>
      <div style="background:${c.primary}; color:#fff; border-radius:10px; padding:14px; text-align:center; font-weight:600; font-size:14px; cursor:pointer;">Add Funds</div>
    </div>
    <div class="app-section">
      <div class="app-section-title" style="color:${textColor}">Recent Activity</div>
      <div style="display:flex; flex-direction:column; gap:10px;">
        <div class="app-service-card" style="background:${c.surface};">
          <div style="width:32px; height:32px; border-radius:50%; background:#22c55e15; display:flex; align-items:center; justify-content:center; font-size:14px; flex-shrink:0;">&#8593;</div>
          <div class="app-service-info">
            <div class="app-service-name" style="color:${textColor}">Added Funds</div>
            <div class="app-service-desc" style="color:${textSecondary}">Feb 1, 2026</div>
          </div>
          <div style="color:#22c55e; font-weight:600; font-size:14px;">+$100.00</div>
        </div>
        <div class="app-service-card" style="background:${c.surface};">
          <div style="width:32px; height:32px; border-radius:50%; background:#ef444415; display:flex; align-items:center; justify-content:center; font-size:14px; flex-shrink:0;">&#8595;</div>
          <div class="app-service-info">
            <div class="app-service-name" style="color:${textColor}">${svc1?.name || 'Service'}</div>
            <div class="app-service-desc" style="color:${textSecondary}">Jan 28, 2026</div>
          </div>
          <div style="color:#ef4444; font-weight:600; font-size:14px;">-$${Math.floor((svc1?.price || 15000) / 100)}.00</div>
        </div>
        <div class="app-service-card" style="background:${c.surface};">
          <div style="width:32px; height:32px; border-radius:50%; background:#f59e0b15; display:flex; align-items:center; justify-content:center; font-size:14px; flex-shrink:0;">&#9733;</div>
          <div class="app-service-info">
            <div class="app-service-name" style="color:${textColor}">Loyalty Bonus</div>
            <div class="app-service-desc" style="color:${textSecondary}">Jan 28, 2026</div>
          </div>
          <div style="color:#f59e0b; font-weight:600; font-size:14px;">+$25.00</div>
        </div>
      </div>
    </div>
    <div style="height: 16px;"></div>
  `;
}

function renderProfileTab(b, c, textColor, textSecondary) {
  return `
    <div style="padding: 48px 16px 8px; text-align:center;">
      <div style="width:64px; height:64px; border-radius:50%; background:${c.primary}; color:#fff; display:flex; align-items:center; justify-content:center; font-size:24px; font-weight:600; margin:0 auto 12px;">S</div>
      <div style="font-size:18px; font-weight:700; color:${textColor}">Sarah Miller</div>
      <div style="font-size:13px; color:${textSecondary}">sarah.miller@email.com</div>
      <div style="display:inline-block; margin-top:6px; font-size:10px; font-weight:600; padding:2px 10px; border-radius:10px; background:#22c55e20; color:#22c55e;">&#10003; Verified</div>
    </div>
    <div class="app-section" style="padding-top:8px;">
      ${[
        { icon: '&#9998;', label: 'Edit Profile' },
        { icon: '&#128276;', label: 'Notifications' },
        { icon: '&#128197;', label: 'Booking History' },
        { icon: '&#128179;', label: 'Payment Methods' },
        { icon: '&#11088;', label: 'Loyalty Program' },
        { icon: '&#128222;', label: 'Contact ${b.name}' },
        { icon: '&#128274;', label: 'Privacy & Security' },
        { icon: '&#128221;', label: 'Terms of Service' },
      ].map(item => `
        <div class="app-service-card" style="background:${c.surface}; cursor:pointer;">
          <div style="width:32px; height:32px; border-radius:8px; background:${c.primary}12; display:flex; align-items:center; justify-content:center; font-size:15px; flex-shrink:0;">${item.icon}</div>
          <div class="app-service-info">
            <div class="app-service-name" style="color:${textColor}; font-size:14px;">${item.label}</div>
          </div>
          <div style="color:${textSecondary}; font-size:14px;">&#10095;</div>
        </div>
      `).join('')}
      <div style="margin-top:16px; background:#ef444415; color:#ef4444; border-radius:10px; padding:14px; text-align:center; font-weight:600; font-size:14px; cursor:pointer;">Log Out</div>
    </div>
    <div style="text-align:center; padding:12px 0 4px; font-size:11px; color:${textSecondary};">v1.0.0 &middot; ${b.name}</div>
    <div style="height: 16px;"></div>
  `;
}

function renderTabBar(b, c, textColor, textSecondary) {
  const tabs = ['home', 'services', 'appointments', 'wallet', 'profile'];
  const labels = ['Home', 'Services', 'Appts', 'Wallet', 'Profile'];

  return `
    <div class="app-tab-bar" style="background:${c.background};">
      ${tabs.map((tab, i) => `
        <div class="app-tab" onclick="switchTab('${tab}')" style="cursor:pointer;">
          <div class="app-tab-icon" style="color:${currentTab === tab ? c.primary : textSecondary}">
            ${getTabIcon(tab)}
          </div>
          <div class="app-tab-label" style="color:${currentTab === tab ? c.primary : textSecondary}">
            ${labels[i]}
          </div>
        </div>
      `).join('')}
    </div>
  `;
}

function switchTab(tab) {
  currentTab = tab;
  renderPhoneScreen();
}

// ─── RENDER INFO PANEL ───
function renderInfoPanel() {
  const b = selectedBusiness;
  if (!b) return;

  const panel = document.getElementById('infoPanel');

  panel.innerHTML = `
    <h3>${b.name}</h3>
    <div class="info-tagline">"${b.tagline}"</div>

    <div class="info-meta">
      <div class="info-meta-item">
        <span class="info-meta-icon">&#128205;</span>
        <span>${b.city}, ${b.state}</span>
      </div>
      <div class="info-meta-item">
        <span class="info-meta-icon">&#127978;</span>
        <span>${b.type}</span>
      </div>
      <div class="info-meta-item">
        <span class="info-meta-icon">&#128222;</span>
        <span>${b.phone}</span>
      </div>
    </div>

    <!-- Why This Business Needs an App -->
    <div class="info-section">
      <div class="info-section-title">Why ${b.name} Needs This App</div>
      <ul class="info-pitch-list">
        ${b.pitchPoints.map(p => `<li>${p}</li>`).join('')}
      </ul>
    </div>

    <!-- App Features -->
    <div class="info-section">
      <div class="info-section-title">App Features Included</div>
      <div class="info-features-grid">
        ${b.appFeatures.map(f => `<span class="info-feature-tag">${f}</span>`).join('')}
      </div>
    </div>

    <!-- Staff -->
    <div class="info-section">
      <div class="info-section-title">Team (${b.staff.length} providers)</div>
      <div class="info-staff-list">
        ${b.staff.map(s => `
          <div class="info-staff-item">
            <div class="info-staff-avatar" style="background:${b.colors.primary}">${s.name.split(' ').map(n => n[0]).join('')}</div>
            <div>
              <div class="info-staff-name">${s.name}</div>
              <div class="info-staff-title">${s.title}</div>
            </div>
          </div>
        `).join('')}
      </div>
    </div>

    <!-- Services Summary -->
    <div class="info-section">
      <div class="info-section-title">${b.services.length} Services Across ${b.categories.length} Categories</div>
      <div class="info-features-grid">
        ${b.categories.map(cat => {
          const count = b.services.filter(s => s.category === cat).length;
          return `<span class="info-feature-tag">${cat} (${count})</span>`;
        }).join('')}
      </div>
    </div>

    <!-- Estimated Impact -->
    <div class="info-section">
      <div class="info-section-title">Estimated Impact</div>
      <div class="info-impact-grid">
        <div class="info-impact-stat">
          <div class="info-impact-val">-40%</div>
          <div class="info-impact-label">Fewer No-Shows</div>
        </div>
        <div class="info-impact-stat">
          <div class="info-impact-val">+30%</div>
          <div class="info-impact-label">More Revenue</div>
        </div>
        <div class="info-impact-stat">
          <div class="info-impact-val">24/7</div>
          <div class="info-impact-label">Booking</div>
        </div>
        <div class="info-impact-stat">
          <div class="info-impact-val">3x</div>
          <div class="info-impact-label">Referrals</div>
        </div>
      </div>
    </div>

    <button class="info-cta-btn" onclick="scrollToSection('pricing')">
      Get This App for ${b.name} &rarr;
    </button>
  `;
}

// ─── UTILITY ───
function isLightColor(hex) {
  const c = hex.replace('#', '');
  const r = parseInt(c.substr(0, 2), 16);
  const g = parseInt(c.substr(2, 2), 16);
  const b = parseInt(c.substr(4, 2), 16);
  return (r * 299 + g * 587 + b * 114) / 1000 > 150;
}

// ─── NAV SCROLL ───
window.addEventListener('scroll', () => {
  document.getElementById('nav').classList.toggle('scrolled', window.scrollY > 40);
});

// ─── MOBILE NAV ───
document.getElementById('navToggle').addEventListener('click', () => {
  document.getElementById('navLinks').classList.toggle('open');
});
document.querySelectorAll('.nav-links a').forEach(a => {
  a.addEventListener('click', () => document.getElementById('navLinks').classList.remove('open'));
});

// ─── SMOOTH SCROLL ───
document.querySelectorAll('a[href^="#"]').forEach(a => {
  a.addEventListener('click', e => {
    e.preventDefault();
    const t = document.querySelector(a.getAttribute('href'));
    if (t) t.scrollIntoView({ behavior: 'smooth', block: 'start' });
  });
});

// ─── SCROLL REVEAL ───
const obs = new IntersectionObserver(entries => {
  entries.forEach(e => {
    if (e.isIntersecting) {
      e.target.classList.add('vis');
      obs.unobserve(e.target);
    }
  });
}, { threshold: 0.08, rootMargin: '0px 0px -40px 0px' });

document.querySelectorAll('.reveal').forEach(el => obs.observe(el));
