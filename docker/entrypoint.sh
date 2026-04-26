#!/usr/bin/env bash
set -euo pipefail

: "${TEST_ENV:=qa}"
: "${TEST_CATEGORY:=Smoke}"
: "${TEST_SUITE:=All}"
: "${BROWSERS:=Chromium}"

echo "==> Environment:    ${TEST_ENV}"
echo "==> Category:       ${TEST_CATEGORY}"
echo "==> Suite:          ${TEST_SUITE}"
echo "==> Browsers:       ${BROWSERS}"

# Map browsers to PWFX_ env vars (one indexed entry each).
IFS=',' read -ra BROWSER_ARR <<< "${BROWSERS}"
for i in "${!BROWSER_ARR[@]}"; do
  export "PWFX_Framework__Browser__Enabled__${i}=${BROWSER_ARR[$i]}"
done

declare -A SUITES=(
  ["UI"]="tests/Tests.UI/Tests.UI.csproj"
  ["Api"]="tests/Tests.Api/Tests.Api.csproj"
  ["E2E"]="tests/Tests.E2E/Tests.E2E.csproj"
  ["Bdd"]="tests/Tests.Bdd/Tests.Bdd.csproj"
)

if [[ "${TEST_SUITE}" == "All" ]]; then
  PROJECTS=("${SUITES[@]}")
else
  PROJECTS=("${SUITES[$TEST_SUITE]}")
fi

FILTER_ARG=()
if [[ "${TEST_CATEGORY}" != "All" ]]; then
  FILTER_ARG=(--filter "Category=${TEST_CATEGORY}")
fi

for proj in "${PROJECTS[@]}"; do
  echo "==> dotnet test ${proj}"
  dotnet test "${proj}" -c Release --no-build \
    --logger trx --logger "console;verbosity=normal" \
    --results-directory TestResults \
    --settings .runsettings \
    "${FILTER_ARG[@]}"
done
