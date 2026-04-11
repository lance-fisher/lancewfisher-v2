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
  },

  "operator-console": {
    title: "Operator Console",
    number: "16",
    tagline: "Unified control plane for the entire Sovereign ecosystem",
    disclosure: "restricted-detail",
    status: "Active",
    statusClass: "active",
    tech: ["Python", "TypeScript", "REST API", "WebSocket", "Local AI"],
    overview: "A unified operator control plane for the entire ecosystem. Provides real-time visibility and control over all running services, AI agents, trading systems, and background workers through a single authenticated interface. Designed as a command center for managing a private technology infrastructure stack running 25+ projects across multiple machines.",
    philosophy: "Managing 25 separate systems through individual terminals and dashboards is untenable at scale. Every service in the ecosystem exposes health endpoints and control surfaces. This console aggregates them all into one authenticated interface with real authority to start, stop, and reconfigure anything. Not a monitoring widget. A command center.",
    capabilities: [
      "Real-time health monitoring across all running services and background workers",
      "Unified control surface for AI agents, trading systems, automation workers, and network services",
      "Authenticated REST API with session management and role-based access",
      "Service registry integration with PROJECTS.json for automatic discovery",
      "Execution control: start, stop, pause, and reconfigure any registered service",
      "Live log streaming from all services with filtering and search"
    ],
    approach: [
      "PROJECTS.json as the service registry for zero-config service discovery",
      "WebSocket-based real-time status streaming for instant health feedback",
      "Authentication layer with session tokens and configurable access policies",
      "Modular adapter architecture: each service integration is a pluggable connector"
    ],
    stats: [
      { value: "25+", label: "Services" },
      { value: "1", label: "Interface" },
      { value: "REST", label: "API" },
      { value: "Auth", label: "Protected" }
    ]
  },

  "session-atlas": {
    title: "Session Atlas",
    number: "17",
    tagline: "Complete map of every AI development session across all projects",
    disclosure: "public-curated",
    status: "Active",
    statusClass: "active",
    tech: ["TypeScript", "React", "Fastify", "SQLite", "JSONL Parsing"],
    overview: "A local-first dashboard that ingests every Claude Code session log and builds a complete navigable map of AI-assisted development work across all projects and time. Search by project, topic, date, or keyword. See the full conversation history, tool use patterns, and work completed in every session. The session record for an entire software ecosystem, queryable in real time.",
    philosophy: "Every hour of AI-assisted development produces a session log that disappears once the context window closes. Session Atlas treats those logs as a durable record of how software actually gets built. Every decision, every correction, every code path explored is in there. This dashboard makes the entire record navigable, searchable, and useful across all 25+ projects.",
    capabilities: [
      "Ingests and indexes JSONL session logs from all Claude Code projects",
      "Full-text search across sessions, projects, topics, and tool use history",
      "Timeline view of all development activity across the entire portfolio",
      "Per-project session breakdown with completion tracking and pattern analysis",
      "81+ sessions indexed across 20+ projects with sub-second search response"
    ],
    approach: [
      "JSONL log ingestion with streaming parse for large session files",
      "SQLite full-text search with indexed project, date, and tool-use columns",
      "React frontend with real-time filtering and timeline visualization",
      "Fastify server at port 8092 with local-only access by default"
    ],
    stats: [
      { value: "81+", label: "Sessions" },
      { value: "20+", label: "Projects" },
      { value: "FTS", label: "Search" },
      { value: "0", label: "Cloud Deps" }
    ]
  },

  "private-intelligence-platform": {
    title: "Private Intelligence Platform",
    number: "18",
    tagline: "Fully local AI workstation — every model on your hardware, zero external APIs",
    disclosure: "public-curated",
    status: "Active",
    statusClass: "active",
    tech: ["TypeScript", "Electron", "Ollama", "React", "RTX 4070"],
    overview: "A full-featured AI workstation built as an Electron desktop app. All inference runs locally on an RTX 4070 via Ollama. Includes a unified chat UI, code assistant, model management dashboard, service health monitoring, and remote phone access. No API keys. No subscriptions. No data leaving the machine.",
    philosophy: "The mainstream AI pitch is convenience at the cost of your data, your privacy, and your independence. This platform rejects that entirely. Every model runs on your own GPU. Every conversation stays on your own hardware. The goal isn't to mirror what cloud AI services offer — it's to build something genuinely sovereign: AI that works for you, not for a company's training pipeline.",
    capabilities: [
      "Unified chat interface across all locally-running Ollama models — no external API calls",
      "Code assistant with project-aware context and multi-file editing support",
      "Model management: pull, update, and remove Ollama models with VRAM monitoring",
      "Service dashboard showing health status of all ecosystem services in real time",
      "Remote phone access for AI queries from iOS routed through the local inference stack"
    ],
    approach: [
      "Electron for native desktop integration with local filesystem and process management",
      "Ollama backend: all inference on local RTX 4070 — zero external model calls ever",
      "Vite client at :5174, Express server at :3500, Ollama at :11434",
      "Phases 0-3 MVP complete; model expansion via Ollama registry ongoing"
    ],
    stats: [
      { value: "100%", label: "Local" },
      { value: "0", label: "External APIs" },
      { value: "RTX", label: "4070 GPU" },
      { value: "Phone", label: "Access" }
    ]
  },

  "llm-enclave": {
    title: "LLM Enclave",
    number: "19",
    tagline: "Hardened zero-trust security enclave for local LLM inference",
    disclosure: "public-curated",
    status: "Active",
    statusClass: "active",
    tech: ["Python", "PowerShell", "Ollama", "Zero-Trust", "Audit Trail"],
    overview: "A hardened security enclave for running local LLM inference with controlled workspace access. The LLM operates inside a policy-gated environment where file access is explicitly whitelisted in a bridge policy. Every file read, every write, every path the model touches is recorded in a hash-chain audit trail. The model sees exactly what you allow it to see, nothing more.",
    philosophy: "When an LLM has access to your filesystem, the question is not whether to trust it, but how to define and enforce the boundaries of that trust. Zero-trust applied to AI means the model starts with no permissions. Access to any workspace file requires an explicit policy grant. Every action is logged in a tamper-evident chain. If something unexpected happens, the audit trail shows exactly what the model touched and when.",
    capabilities: [
      "Zero-trust security model: LLM starts with no file system permissions by default",
      "Policy-gated workspace bridge controlled by bridge_policy.yaml configuration",
      "Hash-chain audit trail providing tamper-evident log of all file access and operations",
      "Explicit path allowlisting: only whitelisted directories and files are accessible to the model",
      "Ollama integration for local model inference within the hardened environment"
    ],
    approach: [
      "Python runtime enforcing policy checks before any file operation is passed to the LLM",
      "PowerShell integration for Windows-native process and filesystem management",
      "YAML-defined bridge policy specifying exact allowed read and write paths per project",
      "Hash-chain implementation recording every operation with cryptographic linkage for tamper detection"
    ],
    stats: [
      { value: "Zero", label: "Trust Model" },
      { value: "Policy", label: "Gated Access" },
      { value: "Hash", label: "Chain Audit" },
      { value: "Local", label: "Inference" }
    ]
  },

  "private-tax-platform": {
    title: "Private Tax & Finance Platform",
    number: "20",
    tagline: "Automated tax prep pipeline for multi-entity filers with trading income",
    disclosure: "public-curated",
    status: "Active",
    statusClass: "active",
    tech: ["Python", "TypeScript", "PDF Processing", "IRS Schemas", "Automation"],
    overview: "A personal tax preparation and financial data system that automates the full pipeline from raw financial data ingestion through form population to final output. Handles multiple entity types, income sources, and deduction categories. Processes brokerage statements, W-2s, 1099s, and Schedule K-1s programmatically. Built for the complexity of trading income, business revenue, and standard filings running simultaneously.",
    philosophy: "Tax software is designed to extract maximum time from the user. A developer with trading income, business revenue, multiple entities, and dozens of 1099s should not be clicking through a wizard for six hours. Tax preparation is a data transformation problem: clean inputs, defined schemas, deterministic outputs, full audit trail.",
    capabilities: [
      "Automated ingestion of brokerage statements, W-2s, 1099s, and Schedule K-1s",
      "Multi-entity filing support across personal and business returns simultaneously",
      "Trading income aggregation from multiple exchanges with wash sale detection",
      "Form population engine targeting standard IRS schedule formats",
      "Validation layer with cross-form consistency checks before final output"
    ],
    approach: [
      "PDF parsing pipeline for structured extraction from financial institution documents",
      "Schema-validated data model for each form type with transformation rules",
      "Trading income processor handling multi-exchange CSV exports and cost basis tracking",
      "Output targeting both human-readable summary and machine-readable formats"
    ],
    stats: [
      { value: "Multi", label: "Entity" },
      { value: "Auto", label: "Ingestion" },
      { value: "IRS", label: "Schemas" },
      { value: "Full", label: "Audit Trail" }
    ]
  },

  "harmony-medspa": {
    title: "Harmony Medspa",
    number: "21",
    tagline: "Full-stack medspa booking platform built for a real business",
    disclosure: "public-safe",
    status: "Active",
    statusClass: "active",
    tech: ["React Native", "Expo", "NestJS", "PostgreSQL", "Stripe"],
    overview: "A production-ready medspa booking platform built for a real med spa. Includes a full-featured mobile app prototype with appointment booking, service catalog, staff profiles, treatment consultation flows, and Stripe payment integration. Part of the FS App Studio portfolio, the live embedded demo shows the complete client experience from discovery through checkout.",
    philosophy: "Every local business deserves a branded app on their clients' home screens. Not a Vagaro listing. Not a generic booking widget. Their app, their brand, their client relationship. Harmony Medspa is the proof of that conviction, live and interactive. The pitch is simple: your clients will book from your app, not a third-party marketplace.",
    capabilities: [
      "Full appointment booking flow with service selection, staff choice, and time slot management",
      "Service catalog with treatment descriptions, pricing, and consultation options",
      "Stripe payment integration for deposits and full service payments",
      "Staff profiles with specialties, certifications, and availability management",
      "Push notifications for appointment reminders and booking confirmations",
      "Client loyalty program with visit tracking and reward redemption"
    ],
    approach: [
      "React Native + Expo for cross-platform mobile deployment (iOS and Android)",
      "NestJS backend with PostgreSQL for appointment and client data management",
      "Stripe Elements for PCI-compliant payment capture",
      "Interactive HTML prototype served from the portfolio for live sales demos"
    ],
    stats: [
      { value: "Live", label: "Demo" },
      { value: "Stripe", label: "Payments" },
      { value: "Full", label: "Booking Flow" },
      { value: "Live", label: "Market" }
    ]
  },

  "sovereign-trade-engine": {
    title: "Sovereign Trade Engine",
    number: "22",
    tagline: "Unified execution layer across six engines and three blockchains",
    disclosure: "restricted-detail",
    status: "Active",
    statusClass: "active",
    tech: ["TypeScript", "ccxt", "ethers.js", "Solana web3.js", "Risk Management"],
    overview: "The execution core of the trading ecosystem. A unified trade execution system that receives signals from the Autonomous Trading Desk and routes orders to the appropriate engine based on asset class, liquidity conditions, and risk parameters. Spans centralized exchanges via ccxt, on-chain Solana DEX trading via Jupiter, and Polymarket CLOB execution. Operates all six engines simultaneously with independent circuit breakers, position limits, and a central kill switch.",
    philosophy: "The Sovereign Trade Engine does not think. It executes. Every order it routes came from a strategy that passed promotion gates in the Autonomous Trading Desk. Its job is to be fast, correct, and safe in that order. The kill switch is always one key press away. DRY_RUN is always the default. Real capital only flows after explicit promotion.",
    capabilities: [
      "Six simultaneous engines: Solana (Jupiter/DexScreener), Polymarket CLOB, Binance, Coinbase, Kraken, ETH DeFi",
      "Signal intake from the Autonomous Trading Desk via internal typed message bus",
      "Independent circuit breakers per engine with configurable loss thresholds",
      "Central kill switch halting all execution instantly across all six engines",
      "Real-time position tracking and P&L reporting across all active venues",
      "DRY_RUN mode by default with explicit promotion required for live execution"
    ],
    approach: [
      "TypeScript with ccxt for standardized CEX connectivity and custom adapters for DeFi",
      "Solana web3.js and Jupiter v6 API for on-chain Solana DEX execution",
      "Event-driven signal intake from the Profit Desk agent via typed message bus",
      "Configurable per-engine position limits, daily loss limits, and slippage thresholds"
    ],
    stats: [
      { value: "6", label: "Engines" },
      { value: "5", label: "Exchanges" },
      { value: "3", label: "Chains" },
      { value: "DRY", label: "Default Mode" }
    ]
  },

  "prediction-market-executor": {
    title: "Prediction Market Executor",
    number: "23",
    tagline: "Autonomous CLOB execution bot for Polymarket prediction markets",
    disclosure: "restricted-detail",
    status: "Paused",
    statusClass: "paused",
    tech: ["JavaScript", "Polymarket CLOB API", "ethers.js", "Polygon", "On-Chain"],
    overview: "An autonomous execution bot purpose-built for Polymarket's Central Limit Order Book. Monitors target market conditions, calculates optimal entry prices against the live order book, and executes positions directly against the CLOB API using a Polygon wallet. Superseded by the Prediction Market Assistant and Sovereign Trade Engine architecture, which subsume this capability with better risk management and ecosystem integration.",
    philosophy: "This was the first serious on-chain execution bot. Direct CLOB access, real Polygon wallet, autonomous position sizing. Everything the later prediction market tools built on. Paused because the Sovereign Trade Engine architecture now subsumes it cleanly, but it earned its place as the first proof that on-chain autonomous execution for prediction markets was viable.",
    capabilities: [
      "Direct Polymarket CLOB API integration for limit and market order placement",
      "Live order book analysis for optimal entry price calculation",
      "Autonomous position sizing based on configured capital allocation rules",
      "Polygon wallet integration via ethers.js for on-chain execution",
      "Target market monitoring with configurable entry condition triggers"
    ],
    approach: [
      "JavaScript with ethers.js for Polygon network interaction and wallet management",
      "Polymarket CLOB REST API for order placement and order book streaming",
      "Configurable strategy parameters for entry thresholds and position sizing",
      "Paper mode for strategy validation before real capital deployment"
    ],
    stats: [
      { value: "CLOB", label: "Order Book" },
      { value: "Polygon", label: "Chain" },
      { value: "Auto", label: "Execution" },
      { value: "Paused", label: "Status" }
    ]
  }

};
