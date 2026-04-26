# ADR 0002 — Reqnroll for BDD

## Status
Accepted

## Context
SpecFlow is no longer maintained. Teams already familiar with Gherkin needed a drop-in replacement.

## Decision
Use **Reqnroll** (community fork of SpecFlow) with the NUnit generator. Pair with `Allure.Reqnroll` for native Allure scenario reporting.

## Consequences
- Identical Gherkin syntax and `[Binding]` model — minimal migration cost.
- Active maintenance, .NET 8 support, MIT-licensed.
- BDD coexists with imperative NUnit tests in the same solution.
