# Retrieval Stack Decision

## Decision

For the thin vertical slice, retrieval uses:

- storage: SQLite
- lexical engine: FTS5 with `bm25(...)`
- ranking bias: authority score plus frontier-artifact priority
- context assembly: heading-aware text chunks

## Why Dense Vector Search Is Not In The Slice

The installed local Ollama inventory does not currently expose an embedding-capable model in a way this slice can rely on safely. The models expose completion and tool capabilities, but not a dedicated local embedding workflow suitable for a grounded dense-retrieval dependency.

The slice will not fake dense retrieval.

## Embedding Decision

- current embedding model: none installed for slice use
- current embedding dimensions: none active
- future preferred local embedding model: a dedicated local embedding model such as `nomic-embed-text` or `bge-m3` once it is actually installed

## Vector Store Decision

- current slice store: SQLite only
- rationale: zero external dependency, ships with Python, stable offline, easy to keep inside the Bloodlines root

## Chunking Strategy

- target chunk size: 850 characters
- hard cap: 1200 characters
- overlap: 160 characters
- structure-aware: split first on markdown headings, then on paragraphs

## Hybrid Retrieval Posture

The slice uses a lexical-plus-authority hybrid rather than vector-plus-BM25:

- FTS5 BM25 ranks the textual match
- authority score boosts authoritative files
- frontier-session artifacts receive an additional priority bump
- context assembly limits the final pack to the highest-signal chunks

## Retrieval Budget

- resume mode context budget: 8 chunks max
- dashboard detail snippets: 5 chunks max
- provenance list: top 5 source files
