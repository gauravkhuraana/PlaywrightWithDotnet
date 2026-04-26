# ADR 0001 — .NET 8 + NUnit

## Status
Accepted

## Context
We needed a long-term-support (LTS) runtime that all enterprise environments support, plus a mature, well-documented test runner.

## Decision
Target **`net8.0`** (LTS until Nov 2026) and use **NUnit 4**. Central Package Management (`Directory.Packages.props`) keeps versions consistent.

## Consequences
- Pros: LTS, broad CI/Docker availability, NUnit's `ParallelScope` model maps cleanly to multi-browser fixtures.
- Cons: Cannot use C# 13-only features (`Lock`, `params Span<T>`, etc.). The `Lock` type was replaced with a private `object` sentinel.
