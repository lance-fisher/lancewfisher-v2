/**
 * Public-safe project metadata for detail pages.
 * Disclosure levels: public-safe | public-curated | restricted-detail
 *
 * public-safe:       Full public explanation with feature detail
 * public-curated:    Polished overview with deliberate abstraction
 * restricted-detail: Limited public context, emphasizes value/philosophy
 */
var PROJECT_DATA = {

  "trading-engine": {
    title: "Multi-Chain Trading Engine",
    number: "04",
    tagline: "Unified execution across exchanges and blockchains",
    disclosure: "restricted-detail",
    status: "Active",
    statusClass: "active",
    tech: ["TypeScript", "WebSocket", "Multi-Exchange", "Real-Time"],
    overview: "A unified execution layer that spans multiple exchanges and blockchain networks through a single operational interface. Designed for autonomous operation with paper-first discipline, meaning every strategy must prove itself in simulation before touching real capital. The system maintains real-time connectivity to all venues simultaneously, streaming live market data and tracking positions across the entire portfolio.",
    philosophy: "Speed, discipline, and brutal honesty. Six engines across five exchanges is not about complexity for its own sake. It is about never being locked into a single venue, never dependent on one API, never exposed to a single point of failure. Paper-first is non-negotiable. Missing a trade is always better than losing capital to an untested strategy.",
    capabilities: [
      "Simultaneous connectivity to multiple centralized and decentralized exchanges",
      "Real-time dashboard streaming live market data and portfolio state",
      "Paper-first discipline with explicit promotion gates before live execution",
      "Cross-chain order execution with sub-second latency on supported venues",
      "AI-assisted strategy analysis and continuous market commentary",
      "Defense-in-depth stop-loss and take-profit enforcement with central safety monitor"
    ],
    approach: [
      "Modular engine architecture where each exchange runs independently",
      "Event-driven design with publish-subscribe for real-time UI updates",
      "Unified abstraction layer for centralized exchanges with custom adapters for DeFi protocols",
      "Every engine defaults to simulation mode with configurable promotion thresholds"
    ],
    stats: [
      { value: "6", label: "Engines" },
      { value: "5", label: "Exchanges" },
      { value: "3", label: "Chains" },
      { value: "24/7", label: "Uptime" }
    ]
  },

  "sovereign-hub": {
    title: "Fisher Sovereign Hub",
    number: "05",
    tagline: "Single pane of glass for a 25+ project portfolio",
    disclosure: "public-curated",
    status: "Active",
    statusClass: "active",
    tech: ["Python", "Git Integration", "JSON Manifest", "Automation"],
    overview: "An operational command center that monitors and synchronizes over twenty-five active projects through a single dashboard. Tracks git status, sync state, and health across the entire portfolio with automatic GitHub synchronization every five minutes. The system treats the entire workspace as a single organism rather than a collection of disconnected repositories.",
    philosophy: "Context-switching between twenty repositories, forgetting what was pushed from a phone, losing track of which project needs attention. These are organizational failures, not technical ones. The Sovereign Hub solves them by treating the entire workspace as a single system. Every repo, every sync, every status check flows through one place.",
    capabilities: [
      "Automatic GitHub synchronization every five minutes across all repositories",
      "Manifest-driven index with status, tags, entry points, and activity tracking",
      "Operational dashboard showing dirty, clean, ahead, and behind status for every repo",
      "One-click manual sync with verbose logging and conflict detection",
      "Full operational runbook documenting daily workflow and recovery procedures"
    ],
    approach: [
      "Lightweight Python server with JSON manifest as the single source of truth",
      "Scheduled synchronization with fast-forward-only pulls and dirty-repo protection",
      "Per-repository configuration for granular control over sync behavior",
      "Structured logging with timestamps, actions taken, and error tracking"
    ],
    stats: [
      { value: "25+", label: "Projects" },
      { value: "5m", label: "Sync Interval" },
      { value: "0", label: "Cloud Deps" },
      { value: "24/7", label: "Uptime" }
    ]
  },

  "trading-desk": {
    title: "Autonomous Trading Desk",
    number: "08",
    tagline: "Multi-agent AI system with rigorous promotion gates",
    disclosure: "restricted-detail",
    status: "Active",
    statusClass: "active",
    tech: ["Python", "Multi-Agent AI", "Strategy Pipeline", "Risk Management"],
    overview: "A multi-agent AI system where six specialized agents collaborate to manage the full lifecycle of trading strategy development. From research and backtesting through risk assessment to execution, each agent has a defined role and accountability. No strategy reaches live markets without passing rigorous quantitative thresholds across weeks of simulated performance.",
    philosophy: "This is where AI stops being a buzzword and starts being accountable. Six agents, each with a specific job, each required to justify its decisions. The promotion gates are intentionally harsh. No strategy goes live without proving itself over weeks of paper trading. Built on the conviction that black boxes should not be trusted with capital.",
    capabilities: [
      "Six specialized agents managing research, backtesting, risk, execution, and monitoring",
      "Multiple independent trading strategies with distinct market approaches",
      "Rigorous promotion pipeline with quantitative thresholds before advancing",
      "Three operating modes with disciplined progression from simulation to live",
      "Complete audit trail for every agent decision and trade action"
    ],
    approach: [
      "Event-driven publish-subscribe architecture for asynchronous agent communication",
      "Each agent runs as an independent process with its own state and decision loop",
      "Configuration-driven risk limits, universe selection, and strategy parameters",
      "Comprehensive journaling for full traceability of every decision"
    ],
    stats: [
      { value: "6", label: "Agents" },
      { value: "4", label: "Strategies" },
      { value: "3", label: "Modes" },
      { value: "Strict", label: "Risk Gates" }
    ]
  },

  "sovereign-signal": {
    title: "Sovereign Signal",
    number: "09",
    tagline: "End-to-end encrypted messaging with zero-knowledge architecture",
    disclosure: "public-curated",
    status: "In Development",
    statusClass: "active",
    tech: ["TypeScript", "End-to-End Encryption", "Zero-Knowledge", "Passwordless Auth"],
    overview: "A peer-to-peer encrypted messaging platform built on Signal-grade cryptography. Messages are encrypted on the sender's device and can only be decrypted by the intended recipient. The server functions purely as a relay, with zero ability to read message content. Authentication is passwordless, using biometrics or hardware security keys instead of traditional credentials.",
    philosophy: "Privacy is not a feature. It is a right. Built on the conviction that people should be able to have a conversation without a corporation reading it. Zero-knowledge architecture means the server literally cannot see the messages. No backdoors, no compromises, no metadata collection. The kind of communication tool that treats its users as sovereign individuals.",
    capabilities: [
      "Signal-grade cryptography with forward secrecy on every message",
      "Passwordless authentication using biometrics or hardware security keys",
      "Zero-knowledge architecture where the server never sees plaintext content",
      "Full peer-to-peer messaging with client-side encrypted key storage",
      "Comprehensive test suite covering cryptographic primitives and protocol flows"
    ],
    approach: [
      "Modular architecture separating cryptographic core, relay server, and client application",
      "Industry-standard cryptographic library for all encryption operations",
      "Minimal-surface relay server that stores only encrypted data blobs",
      "Modern web framework with responsive interface design"
    ],
    stats: [
      { value: "E2EE", label: "Encryption" },
      { value: "X3DH", label: "Key Exchange" },
      { value: "0", label: "Passwords" },
      { value: "P2P", label: "Architecture" }
    ]
  },

  "home-hub": {
    title: "Home Hub",
    number: "10",
    tagline: "Local-first home network monitoring and security dashboard",
    disclosure: "restricted-detail",
    status: "Active",
    statusClass: "active",
    tech: ["TypeScript", "React", "Real-Time Monitoring", "Local-First"],
    overview: "A local-first home automation and network security dashboard that provides complete visibility into every device on the network. Features device discovery, camera feed management, network traffic monitoring, real-time alerting, and secure remote access via HTTPS with API key authentication and dynamic DNS. All data stays on-premises in a local database with zero cloud dependencies.",
    philosophy: "Sovereignty starts at home. Built on the principle that a homeowner should know exactly what is happening on their network without sending that information to a cloud service. Every byte of data stays local, every camera feed streams on the LAN, and every unknown device that joins the network triggers an alert. Remote access uses self-signed TLS and rate-limited API key auth, no third-party tunneling services.",
    capabilities: [
      "Automatic device discovery with vendor identification across the local network",
      "Camera feed management supporting multiple streaming protocols with health monitoring",
      "Network traffic monitoring with per-device bandwidth tracking",
      "Real-time alerting for new devices, camera issues, and bandwidth anomalies",
      "Interactive network topology visualization with device management",
      "Secure remote access via HTTPS with API key authentication, rate limiting, and dynamic DNS"
    ],
    approach: [
      "Modern React frontend with interactive data visualization components",
      "Lightweight server with local database persistence and scheduled discovery scans",
      "Hybrid real-time update strategy combining WebSocket and polling",
      "Fully local architecture with zero external service dependencies"
    ],
    stats: [
      { value: "5", label: "Pages" },
      { value: "4", label: "Alert Types" },
      { value: "3", label: "Collectors" },
      { value: "0", label: "Cloud Deps" }
    ]
  },

  "prediction-market": {
    title: "Prediction Market Assistant",
    number: "12",
    tagline: "AI-powered technical analysis for prediction markets",
    disclosure: "restricted-detail",
    status: "Active",
    statusClass: "active",
    tech: ["JavaScript", "On-Chain Data", "Technical Analysis", "Real-Time"],
    overview: "A real-time technical analysis assistant purpose-built for prediction markets. Combines multiple independent price feeds with a full suite of technical indicators to generate directional signals with confidence scoring. Includes an autonomous component that can mirror the positions of profitable market participants.",
    philosophy: "Prediction markets are the purest form of putting your money where your mouth is. This tool started as a quick terminal script and evolved into a full analysis suite. Reading price oracles directly means no middleman for market data. Decentralized conviction, backed by code.",
    capabilities: [
      "Real-time market data from multiple independent price feed sources",
      "Full technical indicator suite computed locally in real-time",
      "Directional signal generation with confidence scoring",
      "Autonomous position mirroring capability for tracking profitable participants",
      "Live streaming dashboard with color-coded signal output"
    ],
    approach: [
      "Direct on-chain oracle reads for trustless price data",
      "Real-time WebSocket streams for sub-second price updates",
      "Independent execution module for autonomous position management",
      "Originally prototyped on mobile, expanded to full desktop application"
    ],
    stats: [
      { value: "15m", label: "Market Window" },
      { value: "4", label: "Indicators" },
      { value: "2", label: "Price Feeds" },
      { value: "Live", label: "Streaming" }
    ]
  },

  "bookmark-bot": {
    title: "X Bookmark Bot",
    number: "13",
    tagline: "Transforms saved bookmarks into actionable project improvements",
    disclosure: "public-curated",
    status: "Active",
    statusClass: "active",
    tech: ["Python", "Zero Dependencies", "Agent Architecture", "CLI"],
    overview: "A local-first automation tool that transforms saved X/Twitter bookmarks into actionable improvements for existing projects. It intelligently scans codebases, extracts ideas from saved content, matches them to relevant projects, and generates implementation proposals. Nothing happens without explicit approval, and every change is fully traceable with complete rollback capability.",
    philosophy: "The bridge between inspiration and implementation. Dozens of ideas bookmarked every week: threads about new patterns, clever techniques, architecture insights. This bot turns that passive scrolling into real improvements across the entire project portfolio. Zero dependencies, pure standard library, and absolutely nothing happens without explicit approval.",
    capabilities: [
      "Automated pipeline from bookmarked content to matched project improvement proposals",
      "Multiple import modes for maximum flexibility in content ingestion",
      "Three safe implementation modes: branched, isolated workspace, and diff-only",
      "Multi-agent system with specialized research, audit, and patch-building agents",
      "Complete approval gates with full traceability and rollback on every change"
    ],
    approach: [
      "Pure standard library implementation with zero external dependencies",
      "Modular CLI pipeline with clear stage progression",
      "File-based agent communication bus for inter-agent coordination",
      "Read-only project scanning during analysis phase for safety"
    ],
    stats: [
      { value: "0", label: "Dependencies" },
      { value: "7", label: "Agents" },
      { value: "3", label: "Safe Modes" },
      { value: "254", label: "Tests" }
    ]
  },

  "galleon-splash": {
    title: "Galleon Splash Page",
    number: "14",
    tagline: "The original cinematic concept for lancewfisher.com",
    disclosure: "public-safe",
    status: "Shipped",
    statusClass: "shipped",
    tech: ["HTML", "CSS", "Canvas 2D", "Zero Dependencies"],
    overview: "The original cinematic concept design for lancewfisher.com. A single HTML file featuring storm lightning, matrix rain, HUD overlays, scroll-reveal dossier cards, and a dawn color transition. Twelve distinct concept designs were explored before selecting the final minimalist luxury direction used on the current site. Every concept is preserved in the design archive.",
    philosophy: "This is where it all started. A single HTML file with storms, lightning, and a galleon sailing through the rain. Twelve different concepts explored, each one pushing what is possible without a single npm dependency. The final site design was born from the lessons of every concept that came before it.",
    capabilities: [
      "Cinematic visual experience with storm lightning, matrix rain, and parallax effects",
      "Twelve original concept designs exploring different visual directions",
      "Full scroll-reveal dossier with animated stat counters and section transitions",
      "Canvas-rendered effects using pure browser APIs with no JavaScript libraries",
      "Served as the creative foundation for the current site design"
    ],
    approach: [
      "Single self-contained HTML file with inline CSS and JavaScript",
      "RequestAnimationFrame loop for 60fps canvas effects",
      "IntersectionObserver for scroll-triggered reveal animations",
      "All twelve concept variants preserved as a complete design archive"
    ],
    stats: [
      { value: "12", label: "Concepts" },
      { value: "1", label: "HTML File" },
      { value: "0", label: "Dependencies" },
      { value: "60", label: "FPS" }
    ],
    conceptsLink: true
  },

  "auton": {
    title: "Auton",
    number: "15",
    tagline: "Autonomous background worker for portfolio-wide maintenance",
    disclosure: "restricted-detail",
    status: "Active",
    statusClass: "active",
    tech: ["Python", "Local AI", "Multi-Agent", "Workspace Automation"],
    overview: "An autonomous background worker that continuously scans the entire project portfolio, learns from all codebases, and proposes improvements, documentation updates, and maintenance tasks. Runs a local large language model on dedicated GPU hardware so that no project data ever leaves the machine. Multi-agent architecture with six specialized agents, each with defined responsibilities and safety boundaries.",
    philosophy: "The project that watches all other projects. It scans the workspace, builds context from project structure, and proposes improvements. Everything runs on local hardware through a private language model, so nothing ever leaves the machine. Specialized roles, promotion gates, and a kill switch. Simulation by default, real changes only with explicit approval.",
    capabilities: [
      "Continuous scanning of 20+ projects for improvement opportunities",
      "Local large language model inference on dedicated GPU, fully private",
      "Multiple strategy modes for different types of autonomous work",
      "Multi-layered safety model with simulation default and approval gates",
      "Complete audit trail with rollback capability on every proposed change"
    ],
    approach: [
      "Event-driven coordination between specialized agents with async messaging",
      "Persistent journal and state tracking for full audit trail",
      "REST API for health monitoring, task management, and approval workflows",
      "Progressive promotion model from simulation through supervised to autonomous operation"
    ],
    stats: [
      { value: "6", label: "Agents" },
      { value: "25+", label: "Projects" },
      { value: "3", label: "Modes" },
      { value: "GPU", label: "Local AI" }
    ]
  }

};
