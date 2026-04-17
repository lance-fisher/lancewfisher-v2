const VIEW_STORAGE_KEY = "bloodlines.ops.activeView";
const FILTER_STORAGE_KEY = "bloodlines.ops.viewFilter";

function readStorage(key, fallback) {
  try {
    return window.localStorage.getItem(key) || fallback;
  } catch (error) {
    return fallback;
  }
}

function writeStorage(key, value) {
  try {
    if (!value) {
      window.localStorage.removeItem(key);
      return;
    }
    window.localStorage.setItem(key, value);
  } catch (error) {
    // Ignore storage failures and keep the runtime usable.
  }
}

const state = {
  bootstrap: null,
  agent: null,
  agentConsole: null,
  activeView: (() => {
    const stored = readStorage(VIEW_STORAGE_KEY, "console");
    return stored === "dashboard" ? "console" : stored;
  })(),
  viewFilter: readStorage(FILTER_STORAGE_KEY, ""),
  projectFile: null,
};

async function requestJson(url, options = {}) {
  const response = await fetch(url, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });
  const payload = await response.json();
  if (!response.ok) {
    throw new Error(payload.error || "Request failed");
  }
  return payload;
}

function escapeHtml(value) {
  return String(value ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#39;");
}

function showToast(message, tone = "info") {
  const stack = document.getElementById("toastStack");
  if (!stack) {
    return;
  }
  const toast = document.createElement("div");
  toast.className = `toast ${tone}`;
  toast.innerHTML = `<strong>${escapeHtml(message)}</strong>`;
  stack.appendChild(toast);
  window.setTimeout(() => {
    toast.remove();
  }, 3600);
}

async function performAction(action) {
  try {
    await action();
  } catch (error) {
    showToast(error.message || "Action failed.", "error");
  }
}

async function copyTextToClipboard(text, successMessage) {
  const value = String(text || "").trim();
  if (!value) {
    showToast("Nothing available to copy.", "error");
    return;
  }
  try {
    if (navigator.clipboard?.writeText) {
      await navigator.clipboard.writeText(value);
    } else {
      const textarea = document.createElement("textarea");
      textarea.value = value;
      textarea.setAttribute("readonly", "readonly");
      textarea.style.position = "absolute";
      textarea.style.left = "-9999px";
      document.body.appendChild(textarea);
      textarea.select();
      document.execCommand("copy");
      textarea.remove();
    }
    showToast(successMessage, "success");
  } catch (error) {
    showToast(error.message || "Clipboard write failed.", "error");
  }
}

function applyViewFilter() {
  const input = document.getElementById("viewFilterInput");
  const meta = document.getElementById("viewFilterMeta");
  if (!input || !meta) {
    return;
  }
  const query = input.value.trim().toLowerCase();
  state.viewFilter = input.value;
  writeStorage(FILTER_STORAGE_KEY, state.viewFilter);

  const panel = document.querySelector(`.view-panel[data-view-panel="${state.activeView}"]`);
  const targets = panel
    ? Array.from(panel.querySelectorAll(".list-card, .timeline-item, tbody tr, .detail-line, .workspace-card"))
    : [];

  if (!targets.length) {
    meta.textContent = query
      ? `No filterable items are available in ${state.activeView}.`
      : "Filtering applies to the active view only.";
    return;
  }

  let visibleCount = 0;
  targets.forEach((target) => {
    const matches = !query || target.textContent.toLowerCase().includes(query);
    target.classList.toggle("is-filter-hidden", !matches);
    if (matches) {
      visibleCount += 1;
    }
  });

  meta.textContent = query
    ? `Showing ${visibleCount} of ${targets.length} items in ${state.activeView} for "${input.value.trim()}".`
    : `Filtering ${state.activeView} view only.`;
}

function setActiveView(view) {
  state.activeView = view;
  writeStorage(VIEW_STORAGE_KEY, view);
  document.querySelectorAll(".nav-link").forEach((button) => {
    button.classList.toggle("active", button.dataset.view === view);
  });
  document.querySelectorAll(".view-panel").forEach((panel) => {
    panel.classList.toggle("active", panel.dataset.viewPanel === view);
  });
  applyViewFilter();
}

function jumpToView(view, targetId) {
  setActiveView(view);
  window.requestAnimationFrame(() => {
    const target = targetId ? document.getElementById(targetId) : null;
    if (target) {
      target.scrollIntoView({ behavior: "smooth", block: "start" });
    }
  });
}

function renderList(containerId, items, formatter, emptyText) {
  const element = document.getElementById(containerId);
  if (!element) {
    return;
  }
  if (!items || !items.length) {
    element.innerHTML = `<p class="small-copy">${escapeHtml(emptyText)}</p>`;
    return;
  }
  element.innerHTML = items.map(formatter).join("");
}

function renderDetailLines(containerId, pairs, emptyText = "No data available.") {
  const element = document.getElementById(containerId);
  if (!element) {
    return;
  }
  const filtered = pairs.filter((item) => item && item.value !== undefined && item.value !== null);
  if (!filtered.length) {
    element.innerHTML = `<p class="small-copy">${escapeHtml(emptyText)}</p>`;
    return;
  }
  element.innerHTML = filtered
    .map(
      (item) => `
        <div class="detail-line">
          <span>${escapeHtml(item.label)}</span>
          <strong>${escapeHtml(item.value)}</strong>
        </div>
      `
    )
    .join("");
}

function renderMetrics(bootstrap) {
  const metrics = [
    {
      label: "Continuity Health",
      value: bootstrap.status.continuity_health,
      note: bootstrap.resume_state.ambiguous ? "Anchor review required" : "Ready to resume",
    },
    {
      label: "Last Scan",
      value: bootstrap.status.last_scan_time || "Not scanned",
      note: `${bootstrap.source_map.total_files_scanned || 0} files mapped`,
    },
    {
      label: "Changed Files",
      value: bootstrap.change_report.summary?.changed_documents || 0,
      note: "Most recent continuity delta",
    },
    {
      label: "Canonical Sources",
      value: bootstrap.registry.documents?.length || 0,
      note: "Authority-scored subset",
    },
    {
      label: "Frontier Artifacts",
      value: bootstrap.source_map.frontier_artifacts?.length || 0,
      note: "Claude and Codex parity detection",
    },
    {
      label: "Models Online",
      value: bootstrap.model_inventory.models?.length || 0,
      note: bootstrap.model_inventory.status,
    },
  ];

  document.getElementById("metricGrid").innerHTML = metrics
    .map(
      (metric) => `
        <article class="metric-card">
          <p class="eyebrow">${escapeHtml(metric.label)}</p>
          <h3>${escapeHtml(metric.value)}</h3>
          <p class="small-copy">${escapeHtml(metric.note)}</p>
        </article>
      `
    )
    .join("");
}

function renderHeroChips(bootstrap) {
  const pendingDrafts = bootstrap.agent_console?.drafts?.length || 0;
  const chips = [
    `Write posture: ${bootstrap.write_posture.write_posture}`,
    `Changed docs: ${bootstrap.change_report.summary?.changed_documents || 0}`,
    `Conflict watch: ${bootstrap.change_report.summary?.conflict_watch_count || 0}`,
    `Open tasks: ${bootstrap.tasks_board.open_task_count || 0}`,
    `Drafts staged: ${pendingDrafts}`,
    `Degraded: ${(bootstrap.telemetry.degraded_modes.active || []).length}`,
  ];
  document.getElementById("heroChips").innerHTML = chips
    .map((chip) => `<span class="hero-chip">${escapeHtml(chip)}</span>`)
    .join("");
}

function renderResumeAnchor(bootstrap) {
  const anchor = bootstrap.resume_state.effective_anchor || bootstrap.resume_state.anchor;
  const selectionRequired = bootstrap.resume_state.selection_required;
  const hasManualOverride = Boolean(bootstrap.resume_state.selected_override);
  renderDetailLines(
    "resumeAnchorCard",
    anchor
      ? [
          { label: "Label", value: anchor.label },
          { label: "Path", value: anchor.source_path || "n/a" },
          { label: "Time", value: anchor.event_time || "n/a" },
          {
            label: "Status",
            value: selectionRequired
              ? "Selection required"
              : bootstrap.resume_state.selected_override
                ? `Operator selected ${bootstrap.resume_state.selected_override}`
                : "Usable",
          },
        ]
      : [],
    "No last-good-state candidate has been resolved yet."
  );
  document.getElementById("continuityHealth").textContent = bootstrap.status.continuity_health;
  document.getElementById("clearAnchorButton").classList.toggle("hidden", !hasManualOverride);

  const candidates = (bootstrap.resume_state.candidates || []).filter((item) => item.exists);
  renderList(
    "resumeCandidates",
    selectionRequired ? candidates : [],
    (candidate) => `
      <button class="secondary anchor-button" data-candidate-type="${escapeHtml(candidate.candidate_type)}">
        ${escapeHtml(candidate.label)}
      </button>
    `,
    ""
  );

  document.querySelectorAll(".anchor-button").forEach((button) => {
    button.addEventListener("click", () => performAction(() => selectResumeAnchor(button.dataset.candidateType)));
  });
}

function renderOperationalHealth(bootstrap) {
  const posture = bootstrap.write_posture;
  renderDetailLines("operationalHealth", [
    { label: "Write posture", value: posture.write_posture },
    { label: "Active tier", value: posture.active_tier || "locked" },
    { label: "Offline posture", value: bootstrap.status.offline_posture },
    { label: "WSL", value: bootstrap.environment.wsl.available ? "available" : "not active" },
  ]);

  const pill = document.getElementById("writePosturePill");
  pill.textContent = posture.write_posture === "approved_write" ? "Write enabled" : "Read only";
  pill.className = `status-pill ${posture.write_posture === "approved_write" ? "ok" : "muted"}`;
  document.getElementById("writeTierText").textContent = posture.active_tier
    ? `Current tier: ${posture.active_tier}`
    : "Tier locked for project writes.";
}

function renderMemory(bootstrap) {
  const documents = bootstrap.registry.documents || [];
  if (!documents.length) {
    document.getElementById("memoryRegistry").innerHTML =
      '<p class="small-copy">No canonical registry documents available.</p>';
  } else {
    const rows = documents
      .map(
        (doc) => `
          <tr>
            <td>${escapeHtml(doc.path)}</td>
            <td>${escapeHtml(doc.classification)}</td>
            <td>${escapeHtml(doc.topic)}</td>
            <td>${escapeHtml(doc.authority_score)}</td>
            <td>${escapeHtml(doc.change_state)}</td>
          </tr>
        `
      )
      .join("");
    document.getElementById("memoryRegistry").innerHTML = `
      <table>
        <thead>
          <tr>
            <th>Path</th>
            <th>Class</th>
            <th>Topic</th>
            <th>Authority</th>
            <th>Change</th>
          </tr>
        </thead>
        <tbody>${rows}</tbody>
      </table>
    `;
  }

  renderList(
    "discoveredRegistry",
    bootstrap.discovered_registry.documents || [],
    (doc) => `
      <article class="list-card">
        <strong>${escapeHtml(doc.path)}</strong>
        <p class="small-copy">${escapeHtml(doc.classification)} | authority ${escapeHtml(doc.authority_score)}</p>
        <p class="small-copy">${escapeHtml(doc.reason)}</p>
      </article>
    `,
    "No discovered registry documents available."
  );

  renderList(
    "conflictClusters",
    bootstrap.discovered_registry.conflict_clusters || [],
    (cluster) => `
      <article class="list-card">
        <strong>${escapeHtml(cluster.cluster_key)}</strong>
        <p class="small-copy">${escapeHtml(cluster.state)}</p>
        <p class="small-copy">Canonical candidate: ${escapeHtml(cluster.canonical_candidate)}</p>
      </article>
    `,
    "No conflict clusters detected."
  );
}

function renderChangeReport(bootstrap) {
  const report = bootstrap.change_report || {};
  renderDetailLines("changeSummary", [
    { label: "Changed documents", value: report.summary?.changed_documents || 0 },
    { label: "Authoritative updates", value: report.summary?.authoritative_updates || 0 },
    { label: "Frontier updates", value: report.summary?.frontier_session_updates || 0 },
    { label: "Conflict watch", value: report.summary?.conflict_watch_count || 0 },
    { label: "Added", value: report.summary?.added || 0 },
    { label: "Modified", value: report.summary?.modified || 0 },
    { label: "Removed", value: report.summary?.removed || 0 },
  ]);

  renderList(
    "frontierUpdates",
    report.frontier_session_updates || [],
    (item) => `
      <article class="list-card">
        <strong>${escapeHtml(item.path)}</strong>
        <p class="small-copy">${escapeHtml(item.change_type)} | ${escapeHtml(item.provider || "unknown")} | ${escapeHtml(item.artifact_kind || "artifact")}</p>
        <p class="small-copy">${escapeHtml(item.mtime || "n/a")}</p>
      </article>
    `,
    "No new frontier-session artifact changes were detected."
  );

  renderList(
    "highSignalChanges",
    report.high_signal_changes || [],
    (item) => `
      <article class="list-card">
        <strong>${escapeHtml(item.path)}</strong>
        <p class="small-copy">${escapeHtml(item.change_type)} | ${escapeHtml(item.classification || "unknown")} | authority ${escapeHtml(item.authority_score || 0)}</p>
        <p class="small-copy">${escapeHtml(item.reason || "No reason recorded.")}</p>
      </article>
    `,
    "No high-signal changes recorded."
  );

  renderList(
    "authoritativeUpdates",
    report.authoritative_updates || [],
    (item) => `
      <article class="list-card">
        <strong>${escapeHtml(item.path)}</strong>
        <p class="small-copy">${escapeHtml(item.change_type)} | ${escapeHtml(item.topic || "topic unknown")}</p>
        <p class="small-copy">${escapeHtml(item.reason || "No reason recorded.")}</p>
      </article>
    `,
    "No likely-canonical updates recorded."
  );

  renderList(
    "changeConflictWatch",
    report.conflict_watch || [],
    (cluster) => `
      <article class="list-card">
        <strong>${escapeHtml(cluster.cluster_key || "Conflict cluster")}</strong>
        <p class="small-copy">${escapeHtml(cluster.state || "review")}</p>
        <p class="small-copy">Entries: ${escapeHtml((cluster.entries || []).length)}</p>
      </article>
    `,
    "No conflict clusters need review right now."
  );

  renderList(
    "changeHotspots",
    report.hot_spots || [],
    (spot) => `
      <article class="list-card">
        <strong>${escapeHtml(spot.segment)}</strong>
        <p class="small-copy">${escapeHtml(spot.changes)} changed documents</p>
      </article>
    `,
    "No hot spots available yet."
  );
}

function renderTimeline(bootstrap) {
  renderList(
    "timelineEvents",
    bootstrap.timeline || [],
    (event) => `
      <article class="timeline-item">
        <div class="timeline-line">
          <span class="eyebrow">${escapeHtml(event.event_type)}</span>
          <strong>${escapeHtml(event.summary)}</strong>
        </div>
        <p class="small-copy">${escapeHtml(event.created_at)} | doctrine: ${escapeHtml(event.doctrine_check)}</p>
      </article>
    `,
    "No journal events recorded yet."
  );
}

function renderRouting(bootstrap) {
  const routing = bootstrap.configuration.routing_policy || {};
  renderList(
    "routingAssignments",
    Object.entries(routing),
    ([taskType, config]) => `
      <article class="list-card">
        <strong>${escapeHtml(taskType)}</strong>
        <p class="small-copy">Primary: ${escapeHtml(config.primary_model)}</p>
        <p class="small-copy">Fallback: ${escapeHtml(config.fallback_model)}</p>
      </article>
    `,
    "No routing assignments found."
  );
}

function renderTelemetrySummary(bootstrap) {
  const telemetry = bootstrap.telemetry;
  const rows = [
    `Last scan duration: ${telemetry.ingestion.last_scan_duration_seconds || "n/a"}s`,
    `Changed documents: ${telemetry.ingestion.changed_documents || 0}`,
    `Retrieval hit rate: ${telemetry.retrieval.last_hit_rate}`,
    `Write refusals: ${telemetry.writes.refused_count}`,
    `Routing decisions: ${Object.keys(telemetry.routing.decisions_by_task_type || {}).length}`,
    `Degraded active: ${(telemetry.degraded_modes.active || []).join(", ") || "none"}`,
  ];
  renderList(
    "telemetryPanel",
    rows,
    (row) => `<article class="list-card"><p>${escapeHtml(row)}</p></article>`,
    "No telemetry available."
  );
}

function renderTelemetryDrilldown(bootstrap) {
  const telemetry = bootstrap.telemetry_drilldown || {};

  renderList(
    "telemetryTasks",
    telemetry.recent_tasks || [],
    (task) => `
      <article class="list-card">
        <strong>${escapeHtml(task.task_type)} via ${escapeHtml(task.model)}</strong>
        <p class="small-copy">${escapeHtml(task.wall_time_seconds)}s | prompt ${escapeHtml(task.prompt_tokens)} | output ${escapeHtml(task.output_tokens)}</p>
        <p class="small-copy">retrieval hits ${escapeHtml(task.retrieval_hits)}${task.degraded_reason ? ` | degraded ${escapeHtml(task.degraded_reason)}` : ""}</p>
      </article>
    `,
    "No recent local agent tasks recorded."
  );

  const retrievalCards = [];
  Object.entries(telemetry.retrieval?.top_result_provenance_distribution || {}).forEach(([path, count]) => {
    retrievalCards.push({ title: path, detail: `${count} top hits` });
  });
  (telemetry.retrieval?.recent_queries || []).forEach((query) => {
    retrievalCards.push({ title: query.query, detail: `${query.hit_count} hits at ${query.at}` });
  });
  renderList(
    "telemetryRetrieval",
    retrievalCards,
    (item) => `
      <article class="list-card">
        <strong>${escapeHtml(item.title)}</strong>
        <p class="small-copy">${escapeHtml(item.detail)}</p>
      </article>
    `,
    "No retrieval telemetry recorded yet."
  );

  const writeItems = [
    {
      title: "Summary",
      detail: `approved ${telemetry.writes?.summary?.approved_count || 0} | refused ${telemetry.writes?.summary?.refused_count || 0} | last reason ${telemetry.writes?.summary?.last_reason_code || "n/a"}`,
    },
    ...(telemetry.writes?.recent_events || []).map((event) => ({
      title: event.target_path,
      detail: `${event.approval_state} | ${event.reason_code} | ${event.created_at}`,
    })),
  ];
  renderList(
    "telemetryWrites",
    writeItems,
    (item) => `
      <article class="list-card">
        <strong>${escapeHtml(item.title)}</strong>
        <p class="small-copy">${escapeHtml(item.detail)}</p>
      </article>
    `,
    "No write-gate events recorded."
  );

  const degradedItems = [
    {
      title: "Active",
      detail: (telemetry.degraded_modes?.active || []).join(", ") || "none",
    },
    ...Object.entries(telemetry.degraded_modes?.reasons || {}).map(([reason, count]) => ({
      title: reason,
      detail: `${count} activations`,
    })),
  ];
  renderList(
    "telemetryDegraded",
    degradedItems,
    (item) => `
      <article class="list-card">
        <strong>${escapeHtml(item.title)}</strong>
        <p class="small-copy">${escapeHtml(item.detail)}</p>
      </article>
    `,
    "No degraded-mode history recorded."
  );

  const routingItems = [
    ...Object.entries(telemetry.routing?.decisions_by_task_type || {}).map(([taskType, count]) => ({
      title: taskType,
      detail: `${count} routing decisions`,
    })),
    ...Object.entries(telemetry.routing?.last_assignments || {}).map(([taskType, config]) => ({
      title: `${taskType} assignment`,
      detail: `${config.primary_model} -> ${config.fallback_model}`,
    })),
  ];
  renderList(
    "telemetryRouting",
    routingItems,
    (item) => `
      <article class="list-card">
        <strong>${escapeHtml(item.title)}</strong>
        <p class="small-copy">${escapeHtml(item.detail)}</p>
      </article>
    `,
    "No routing telemetry recorded."
  );

  renderList(
    "telemetryScans",
    telemetry.recent_scans || [],
    (scan) => `
      <article class="list-card">
        <strong>${escapeHtml(scan.mode)} | ${escapeHtml(scan.status)}</strong>
        <p class="small-copy">${escapeHtml(scan.started_at)} -> ${escapeHtml(scan.completed_at || "running")}</p>
        <p class="small-copy">${escapeHtml(scan.files_scanned)} files | ${escapeHtml(scan.subset_docs_ingested)} canonical docs</p>
      </article>
    `,
    "No scan history recorded."
  );
}

function renderModels(bootstrap) {
  renderList(
    "modelInventory",
    bootstrap.model_inventory.models || [],
    (model) => `
      <article class="list-card">
        <strong>${escapeHtml(model.name)}</strong>
        <p class="small-copy">Context: ${escapeHtml(model.context_window || "unknown")}</p>
        <p class="small-copy">Capabilities: ${escapeHtml((model.capabilities || []).join(", ") || "unknown")}</p>
        <p class="small-copy">Roles: ${escapeHtml((model.routing_roles || []).join(", ") || "unassigned")}</p>
      </article>
    `,
    "Ollama inventory is unavailable."
  );
}

function renderEnvironment(bootstrap) {
  const env = bootstrap.environment;
  const wslLines = (env.wsl.running_distributions || [])
    .map((line) => `<p class="small-copy">${escapeHtml(line)}</p>`)
    .join("");
  document.getElementById("environmentPanel").innerHTML = `
    <article class="list-card">
      <strong>Python</strong>
      <p class="small-copy">${escapeHtml(env.python_version)}</p>
    </article>
    <article class="list-card">
      <strong>WSL</strong>
      <p class="small-copy">${escapeHtml(env.wsl.available ? "available" : "not active")}</p>
      ${wslLines}
    </article>
  `;
}

function renderDegraded(bootstrap) {
  const banner = document.getElementById("degradedBanner");
  const active = bootstrap.telemetry.degraded_modes.active || [];
  if (!active.length) {
    banner.classList.add("hidden");
    banner.innerHTML = "";
    return;
  }
  banner.classList.remove("hidden");
  banner.innerHTML = `
    <strong>Degraded mode active:</strong>
    <span>${escapeHtml(active.join(", "))}</span>
  `;
}

function renderAgent(result) {
  const status = document.getElementById("agentStatus");
  if (!result) {
    status.textContent = "No resume run yet in this session.";
    return;
  }
  status.textContent =
    result.status === "requires_anchor_selection"
      ? result.message
      : `Mode: ${result.mode} | confidence: ${result.confidence}`;
  document.getElementById("agentSummary").textContent =
    result.reasoning_summary || result.message || "No agent summary available.";
  document.getElementById("agentNextStep").textContent =
    result.recommended_next_step || "No next step generated yet.";
  renderList(
    "agentUnresolved",
    result.unresolved_items || [],
    (item) => `<article class="list-card"><p>${escapeHtml(item)}</p></article>`,
    "No unresolved items surfaced."
  );
  renderList(
    "agentProvenance",
    result.provenance || [],
    (path) => `<article class="list-card"><p>${escapeHtml(path)}</p></article>`,
    "No provenance recorded yet."
  );
}

function renderAgentConsole(consoleState) {
  state.agentConsole = consoleState;
  const messagesElement = document.getElementById("consoleMessages");
  if (!messagesElement || !consoleState) {
    return;
  }

  const messages = consoleState.session?.messages || [];
  messagesElement.innerHTML = messages
    .map((message) => {
      const routingBits = [];
      if (message.routing?.command) {
        routingBits.push(message.routing.command);
      }
      if (message.routing?.model) {
        routingBits.push(message.routing.model);
      }
      if (message.routing?.task_type) {
        routingBits.push(message.routing.task_type);
      }
      if (message.routing?.tool_steps) {
        routingBits.push(`${message.routing.tool_steps} tool steps`);
      }
      if (message.confidence !== null && message.confidence !== undefined) {
        routingBits.push(`confidence ${message.confidence}`);
      }

      const actions = (message.actions_taken || [])
        .map((item) => `<li>${escapeHtml(item)}</li>`)
        .join("");
      const citations = (message.citations || [])
        .map((item) => `<span class="console-citation">${escapeHtml(item)}</span>`)
        .join("");
      const draftIds = (message.draft_ids || [])
        .map((item) => `<span class="console-draft-pill">${escapeHtml(item)}</span>`)
        .join("");
      const unresolved = (message.unresolved_items || [])
        .map((item) => `<li>${escapeHtml(item)}</li>`)
        .join("");

      return `
        <article class="console-message ${escapeHtml(message.role)} ${escapeHtml(message.kind)}">
          <div class="console-message-meta">
            <span class="console-role">${escapeHtml(message.role)}</span>
            <span>${escapeHtml(message.created_at || "")}</span>
            ${routingBits.length ? `<span>${escapeHtml(routingBits.join(" | "))}</span>` : ""}
          </div>
          <div class="console-message-body">${escapeHtml(message.content || "")}</div>
          ${actions ? `<div class="console-message-section"><p class="eyebrow">Actions Taken</p><ul class="console-inline-list">${actions}</ul></div>` : ""}
          ${unresolved ? `<div class="console-message-section"><p class="eyebrow">Unresolved</p><ul class="console-inline-list">${unresolved}</ul></div>` : ""}
          ${citations ? `<div class="console-message-section"><p class="eyebrow">Citations</p><div class="console-citation-row">${citations}</div></div>` : ""}
          ${draftIds ? `<div class="console-message-section"><p class="eyebrow">Drafts</p><div class="console-citation-row">${draftIds}</div></div>` : ""}
        </article>
      `;
    })
    .join("");

  messagesElement.scrollTop = messagesElement.scrollHeight;

  renderDetailLines("consoleStatusSummary", [
    { label: "Anchor", value: consoleState.status?.anchor_label || "unresolved" },
    { label: "Selection required", value: consoleState.status?.selection_required ? "yes" : "no" },
    { label: "Write posture", value: consoleState.write_posture?.write_posture || "read_only" },
    { label: "Active tier", value: consoleState.write_posture?.active_tier || "locked" },
    { label: "Pending drafts", value: consoleState.status?.pending_drafts || 0 },
    { label: "Next step", value: consoleState.status?.recommended_next_step || "not generated" },
  ]);

  const latestAssistant = [...messages].reverse().find((message) => message.role === "assistant");
  renderList(
    "consoleLatestCitations",
    latestAssistant?.citations || [],
    (item) => `<article class="list-card"><p>${escapeHtml(item)}</p></article>`,
    "The latest assistant turn has no citations yet."
  );

  renderList(
    "consoleSuggestedPrompts",
    consoleState.suggested_prompts || [],
    (item) => `
      <article class="list-card prompt-card">
        <p>${escapeHtml(item)}</p>
        <button class="secondary compact-action console-prime-button" data-prompt="${escapeHtml(item)}">Use Prompt</button>
      </article>
    `,
    "No suggested prompts are available."
  );

  renderList(
    "consoleDrafts",
    consoleState.drafts || [],
    (draft) => `
      <article class="list-card draft-card">
        <strong>${escapeHtml(draft.relative_path)}</strong>
        <p class="small-copy">${escapeHtml(draft.id)} | required ${escapeHtml(draft.preview?.required_tier || "tier_3")}</p>
        <p class="small-copy">${escapeHtml(draft.reason || "No reason recorded.")}</p>
        <pre class="draft-diff-preview">${escapeHtml((draft.preview?.diff_preview || "No diff available.").slice(0, 1600))}</pre>
        <div class="inline-actions inline-actions-tight">
          <button class="primary compact-action console-apply-draft-button" data-draft-id="${escapeHtml(draft.id)}">Apply Draft</button>
          <button class="secondary compact-action console-dismiss-draft-button" data-draft-id="${escapeHtml(draft.id)}">Dismiss</button>
        </div>
      </article>
    `,
    "No governed write drafts are staged."
  );

  document.querySelectorAll(".console-prime-button").forEach((button) => {
    button.addEventListener("click", () => {
      const input = document.getElementById("consoleInput");
      input.value = button.dataset.prompt || "";
      input.focus();
    });
  });

  document.querySelectorAll(".console-apply-draft-button").forEach((button) => {
    button.addEventListener("click", () => performAction(() => applyConsoleDraft(button.dataset.draftId)));
  });

  document.querySelectorAll(".console-dismiss-draft-button").forEach((button) => {
    button.addEventListener("click", () => performAction(() => dismissConsoleDraft(button.dataset.draftId)));
  });

  const commandHints = (consoleState.available_commands || [])
    .slice(0, 8)
    .map(
      (item) =>
        `<button class="secondary compact-action console-command-button" data-command="${escapeHtml(item.command)}">${escapeHtml(item.command)}</button>`
    )
    .join("");
  document.getElementById("consoleCommandHints").innerHTML = commandHints;
  document.querySelectorAll(".console-command-button").forEach((button) => {
    button.addEventListener("click", () => {
      const input = document.getElementById("consoleInput");
      input.value = button.dataset.command || "";
      input.focus();
    });
  });
}

async function submitConsoleMessage() {
  const input = document.getElementById("consoleInput");
  const sendButton = document.getElementById("consoleSendButton");
  const message = input.value.trim();
  if (!message) {
    showToast("Enter a prompt or command first.", "error");
    return;
  }
  input.disabled = true;
  sendButton.disabled = true;
  try {
    const payload = await requestJson("/api/agent-console/message", {
      method: "POST",
      body: JSON.stringify({ message }),
    });
    input.value = "";
    renderAgentConsole(payload);
    await refreshBootstrap(false);
    setActiveView("console");
    showToast("Local Bloodlines agent turn completed.", "success");
  } finally {
    input.disabled = false;
    sendButton.disabled = false;
    input.focus();
  }
}

async function resetConsole() {
  const payload = await requestJson("/api/agent-console/reset", {
    method: "POST",
    body: JSON.stringify({}),
  });
  renderAgentConsole(payload);
  await refreshBootstrap(false);
  setActiveView("console");
  showToast("Console thread reset.", "success");
}

async function applyConsoleDraft(draftId) {
  const payload = await requestJson("/api/agent-console/apply-draft", {
    method: "POST",
    body: JSON.stringify({ draft_id: draftId }),
  });
  renderAgentConsole(payload.agent_console);
  await refreshBootstrap(true);
  setActiveView("console");
  showToast(`Applied draft ${draftId}.`, "success");
}

async function dismissConsoleDraft(draftId) {
  const payload = await requestJson("/api/agent-console/dismiss-draft", {
    method: "POST",
    body: JSON.stringify({ draft_id: draftId }),
  });
  renderAgentConsole(payload.agent_console);
  await refreshBootstrap(false);
  setActiveView("console");
  showToast(`Dismissed draft ${draftId}.`, "success");
}

function renderExecutionPacket(bootstrap) {
  const packet = bootstrap.execution_packet || {};
  renderDetailLines("executionSummary", [
    { label: "Lane", value: packet.execution_lane || "unresolved" },
    { label: "Scene target", value: packet.scene_target || "unresolved" },
    { label: "Recommended next step", value: packet.recommended_next_step || "not generated" },
    { label: "Authoritative sources", value: packet.authoritative_source_count || 0 },
    { label: "Frontier artifacts", value: packet.frontier_artifact_count || 0 },
    { label: "Latest Unity handoff", value: packet.latest_unity_handoff || "none" },
  ]);

  renderList(
    "executionVerifiedState",
    packet.current_verified_state || [],
    (item) => `<article class="list-card"><p>${escapeHtml(item)}</p></article>`,
    "No verified-state packet is available yet."
  );

  renderList(
    "executionPriority",
    packet.project_work_priority || [],
    (item) => `<article class="list-card"><p>${escapeHtml(item)}</p></article>`,
    "No current project-work priority is available."
  );

  renderList(
    "executionManualChecklist",
    packet.manual_verification_checklist || [],
    (item) => `<article class="list-card"><p>${escapeHtml(item)}</p></article>`,
    "No manual verification checklist is available."
  );

  renderList(
    "executionValidationCommands",
    packet.validation_commands || [],
    (item) => `<article class="list-card"><p>${escapeHtml(item)}</p></article>`,
    "No governed validation commands are available."
  );

  renderList(
    "executionCanonicalSources",
    packet.canonical_sources || [],
    (item) => `<article class="list-card"><p>${escapeHtml(item)}</p></article>`,
    "No canonical source list is available."
  );
}

function renderTasksBoard(bootstrap) {
  const board = bootstrap.tasks_board || {};
  renderList(
    "openTasks",
    board.open_tasks || [],
    (item) => `
      <article class="list-card">
        <strong>${escapeHtml(item.task)}</strong>
        <p class="small-copy">${escapeHtml(item.heading)}</p>
      </article>
    `,
    "No open tasks parsed from todo.md."
  );

  renderList(
    "handoffPriorities",
    board.handoff_priority || [],
    (item) => `<article class="list-card"><p>${escapeHtml(item)}</p></article>`,
    "No handoff priority list available."
  );

  document.getElementById("taskRecommendation").textContent =
    board.project_work_priority?.[0] ||
    board.recommended_next_step ||
    "No agent recommendation has been persisted into the task board yet.";
}

function renderHandoff(bootstrap) {
  const builder = bootstrap.handoff_builder || {};
  const preview = bootstrap.handoff_preview || {};

  renderDetailLines("handoffBuilderSummary", [
    { label: "Resume anchor", value: builder.anchor?.label || "unresolved" },
    { label: "Changed documents", value: builder.continuity_delta?.summary?.changed_documents || 0 },
    { label: "Open tasks", value: builder.open_work?.open_tasks?.length || 0 },
    { label: "Project priority", value: builder.open_work?.project_work_priority?.[0] || "none" },
  ]);

  renderList(
    "handoffBuilderBriefing",
    builder.briefing_lines || [],
    (item) => `<article class="list-card"><p>${escapeHtml(item)}</p></article>`,
    "No re-entry briefing lines available."
  );

  renderList(
    "handoffCanonicalSources",
    builder.canonical_sources || [],
    (doc) => `
      <article class="list-card">
        <strong>${escapeHtml(doc.path)}</strong>
        <p class="small-copy">${escapeHtml(doc.classification)} | authority ${escapeHtml(doc.authority_score)}</p>
      </article>
    `,
    "No canonical sources available."
  );

  renderList(
    "handoffEscalations",
    builder.frontier_escalations || [],
    (item) => `
      <article class="list-card">
        <strong>${escapeHtml(item.reason)}</strong>
        <p class="small-copy">${escapeHtml(item.mode || "unspecified")}</p>
      </article>
    `,
    "No frontier escalations are currently queued."
  );

  document.getElementById("handoffPrompt").textContent =
    builder.suggested_prompt || "No handoff prompt has been assembled yet.";
  document.getElementById("handoffPreview").textContent =
    preview.content || "No handoff preview has been generated yet. Use Export Handoff to build one.";
}

function renderBootstrap(bootstrap) {
  state.bootstrap = bootstrap;
  state.agentConsole = bootstrap.agent_console || state.agentConsole;
  const filterInput = document.getElementById("viewFilterInput");
  if (filterInput && filterInput.value !== state.viewFilter) {
    filterInput.value = state.viewFilter;
  }
  document.getElementById("heroSubtitle").textContent = bootstrap.resume_state.selection_required
    ? "Multiple recent resume anchors were detected. Review before committing new Bloodlines work through the local console."
    : "The local agent can now search canon, read files, draft governed updates, and continue Bloodlines offline from this screen.";

  renderHeroChips(bootstrap);
  renderMetrics(bootstrap);
  renderResumeAnchor(bootstrap);
  renderOperationalHealth(bootstrap);
  renderList(
    "recentChanges",
    bootstrap.source_map.recent_changes || [],
    (item) => `
      <article class="list-card">
        <strong>${escapeHtml(item.path)}</strong>
        <p class="small-copy">${escapeHtml(item.mtime)}</p>
        <p class="small-copy">${escapeHtml(item.classification)}</p>
      </article>
    `,
    "No recent changes found."
  );
  renderList(
    "frontierArtifacts",
    bootstrap.source_map.frontier_artifacts || [],
    (item) => `
      <article class="list-card">
        <strong>${escapeHtml(item.path)}</strong>
        <p class="small-copy">${escapeHtml(item.provider)} | ${escapeHtml(item.artifact_kind)}</p>
        <p class="small-copy">${escapeHtml(item.mtime)}</p>
      </article>
    `,
    "No frontier artifacts detected in the current map."
  );
  renderMemory(bootstrap);
  renderChangeReport(bootstrap);
  renderExecutionPacket(bootstrap);
  renderTasksBoard(bootstrap);
  renderTimeline(bootstrap);
  renderRouting(bootstrap);
  renderTelemetrySummary(bootstrap);
  renderTelemetryDrilldown(bootstrap);
  renderModels(bootstrap);
  renderEnvironment(bootstrap);
  renderHandoff(bootstrap);
  renderDegraded(bootstrap);
  renderAgentConsole(state.agentConsole);

  if (bootstrap.last_agent_result && Object.keys(bootstrap.last_agent_result).length) {
    state.agent = bootstrap.last_agent_result;
    renderAgent(state.agent);
  } else {
    renderAgent(state.agent);
  }
  setActiveView(state.activeView);
}

async function refreshBootstrap(force = false) {
  const data = force
    ? await requestJson("/api/rescan", { method: "POST", body: JSON.stringify({ force: true }) })
    : await requestJson("/api/bootstrap");
  renderBootstrap(data);
}

async function runResume() {
  const result = await requestJson("/api/agent/resume", {
    method: "POST",
    body: JSON.stringify({ mode: "resume_last_good_state" }),
  });
  state.agent = result;
  await refreshBootstrap(false);
  renderAgent(result);
  setActiveView("agent");
  showToast(
    result.status === "requires_anchor_selection"
      ? result.message || "Resume requires operator anchor selection."
      : "Resume state refreshed in Agent Workspace.",
    result.status === "requires_anchor_selection" ? "info" : "success"
  );
}

async function selectResumeAnchor(candidateType) {
  await requestJson("/api/select-anchor", {
    method: "POST",
    body: JSON.stringify({ candidate_type: candidateType }),
  });
  await refreshBootstrap(false);
  showToast(
    candidateType ? `Manual resume anchor set to ${candidateType}.` : "Manual resume anchor cleared.",
    "success"
  );
}

async function unlockSession() {
  const phrase = document.getElementById("unlockInput").value;
  if (!phrase.trim()) {
    return;
  }
  try {
    await requestJson("/api/unlock", {
      method: "POST",
      body: JSON.stringify({ phrase }),
    });
    document.getElementById("unlockInput").value = "";
    await refreshBootstrap(false);
    showToast("Write posture unlocked for this session.", "success");
  } catch (error) {
    showToast(error.message, "error");
  }
}

async function exportHandoff() {
  const result = await requestJson("/api/export-handoff", {
    method: "POST",
    body: JSON.stringify({}),
  });
  await refreshBootstrap(false);
  setActiveView("handoff");
  showToast(`Handoff pack written to ${result.path}`, "success");
}

async function loadProjectFile() {
  const relativePath = document.getElementById("projectWritePath").value.trim();
  if (!relativePath) {
    showToast("Enter a relative path first.", "error");
    return;
  }
  const payload = await requestJson("/api/project-file", {
    method: "POST",
    body: JSON.stringify({ relative_path: relativePath }),
  });
  state.projectFile = payload;
  document.getElementById("projectWriteCurrent").value = payload.content || "";
  document.getElementById("projectWriteProposed").value = payload.content || "";
  document.getElementById("projectWritePreview").textContent =
    `Loaded ${payload.relative_path}\nexists=${payload.exists}\nrequired_tier=${payload.required_tier}\nsha256=${payload.sha256 || "new file"}`;
  showToast(`Loaded ${payload.relative_path}`, "success");
}

async function previewProjectWrite() {
  const relativePath = document.getElementById("projectWritePath").value.trim();
  if (!relativePath) {
    showToast("Enter a relative path first.", "error");
    return;
  }
  const payload = await requestJson("/api/project-write/preview", {
    method: "POST",
    body: JSON.stringify({
      relative_path: relativePath,
      content: document.getElementById("projectWriteProposed").value,
      reason: document.getElementById("projectWriteReason").value.trim(),
      source_basis: document.getElementById("projectWriteSourceBasis").value.trim() || "continuation-platform/ui",
    }),
  });
  state.projectFile = {
    ...(state.projectFile || {}),
    relative_path: payload.relative_path,
    sha256: payload.current_sha256,
  };
  document.getElementById("projectWritePreview").textContent = [
    `path=${payload.relative_path}`,
    `required_tier=${payload.required_tier}`,
    `changed=${payload.changed}`,
    `current_sha256=${payload.current_sha256 || "new file"}`,
    `proposed_sha256=${payload.proposed_sha256}`,
    `current_lines=${payload.current_line_count}`,
    `proposed_lines=${payload.proposed_line_count}`,
    `added_lines=${payload.added_line_count}`,
    `removed_lines=${payload.removed_line_count}`,
    "",
    payload.diff_preview,
  ].join("\n");
  showToast(`Write preview generated for ${payload.relative_path}`, "success");
}

async function applyProjectWrite() {
  const relativePath = document.getElementById("projectWritePath").value.trim();
  if (!relativePath) {
    showToast("Enter a relative path first.", "error");
    return;
  }
  const payload = await requestJson("/api/project-write", {
    method: "POST",
    body: JSON.stringify({
      relative_path: relativePath,
      content: document.getElementById("projectWriteProposed").value,
      reason: document.getElementById("projectWriteReason").value.trim(),
      source_basis: document.getElementById("projectWriteSourceBasis").value.trim() || "continuation-platform/ui",
      expected_sha256: state.projectFile?.sha256 || null,
    }),
  });
  await loadProjectFile();
  await refreshBootstrap(true);
  document.getElementById("projectWritePreview").textContent = [
    `Write applied to ${payload.target_path}`,
    `required_tier=${payload.required_tier}`,
    `backup_dir=${payload.backup_dir}`,
    `written_sha256=${payload.written_sha256}`,
  ].join("\n");
  showToast(`Write applied to ${payload.target_path}`, "success");
}

document.querySelectorAll(".nav-link").forEach((button) => {
  button.addEventListener("click", () => setActiveView(button.dataset.view));
});

document.querySelectorAll(".quick-jump-button").forEach((button) => {
  button.addEventListener("click", () => jumpToView(button.dataset.viewTarget, button.dataset.scrollTarget));
});

document.getElementById("viewFilterInput").addEventListener("input", applyViewFilter);
document.getElementById("consoleSendButton").addEventListener("click", () => performAction(submitConsoleMessage));
document.getElementById("consoleResetButton").addEventListener("click", () => performAction(resetConsole));
document.getElementById("consoleResumeButton").addEventListener("click", () => performAction(runResume));
document.getElementById("consoleRescanButton").addEventListener("click", () =>
  performAction(async () => {
    await refreshBootstrap(true);
    showToast("Bloodlines continuity rescan completed.", "success");
  })
);
document.getElementById("consoleInput").addEventListener("keydown", (event) => {
  if (event.key === "Enter" && !event.shiftKey) {
    event.preventDefault();
    performAction(submitConsoleMessage);
  }
});
document.getElementById("clearAnchorButton").addEventListener("click", () => performAction(() => selectResumeAnchor(null)));
document.getElementById("copyNextStepButton").addEventListener("click", () =>
  performAction(() =>
    copyTextToClipboard(document.getElementById("agentNextStep").textContent, "Copied next-step recommendation.")
  )
);
document.getElementById("copyHandoffPromptButton").addEventListener("click", () =>
  performAction(() =>
    copyTextToClipboard(document.getElementById("handoffPrompt").textContent, "Copied handoff prompt.")
  )
);
document.getElementById("copyHandoffPreviewButton").addEventListener("click", () =>
  performAction(() =>
    copyTextToClipboard(document.getElementById("handoffPreview").textContent, "Copied handoff preview.")
  )
);
document.getElementById("rescanButton").addEventListener("click", () =>
  performAction(async () => {
    await refreshBootstrap(true);
    showToast("Bloodlines continuity rescan completed.", "success");
  })
);
document.getElementById("resumeButton").addEventListener("click", () => performAction(runResume));
document.getElementById("handoffButton").addEventListener("click", () => performAction(exportHandoff));
document.getElementById("handoffRebuildButton").addEventListener("click", () => performAction(exportHandoff));
document.getElementById("unlockButton").addEventListener("click", () => performAction(unlockSession));
document.getElementById("projectFileLoadButton").addEventListener("click", () => performAction(loadProjectFile));
document.getElementById("projectWritePreviewButton").addEventListener("click", () => performAction(previewProjectWrite));
document.getElementById("projectWriteApplyButton").addEventListener("click", () => performAction(applyProjectWrite));

document.getElementById("viewFilterInput").value = state.viewFilter;
setActiveView(state.activeView);
refreshBootstrap(false).catch((error) => {
  document.getElementById("heroSubtitle").textContent = error.message;
  showToast(error.message, "error");
});
