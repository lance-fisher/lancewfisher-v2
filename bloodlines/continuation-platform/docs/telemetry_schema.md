# Telemetry Schema

## Required Counters

- `scan_runs_total`
- `scan_duration_ms_last`
- `scan_changed_files_last`
- `routing_decisions_total`
- `routing_by_task_type`
- `agent_resume_runs_total`
- `agent_resume_failures_total`
- `retrieval_queries_total`
- `retrieval_top_sources_last`
- `retrieval_hit_count_last`
- `write_attempts_total`
- `write_refusals_tier_insufficient_total`
- `degraded_mode_total`
- `degraded_mode_last_reason`

## Per-Task Fields

- `task_type`
- `started_at`
- `finished_at`
- `elapsed_ms`
- `model_selected`
- `provider`
- `sources_used`
- `confidence`
- `escalation_required`

## Dashboard Surfaces

- last scan time
- last scan duration
- changed files count
- current degraded mode if any
- current routing models
- current write posture
- current resume candidate
