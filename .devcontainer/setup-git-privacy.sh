#!/usr/bin/env bash

set -euo pipefail

readonly BIN_DIRECTORY="${HOME}/.local/bin"
readonly GIT_WRAPPER="${BIN_DIRECTORY}/git"

mkdir -p "${BIN_DIRECTORY}"

cat > "${GIT_WRAPPER}" <<'EOF'
#!/usr/bin/env bash

set -euo pipefail

readonly REAL_GIT="/usr/bin/git"
readonly PRIVACY_TIMEZONE="America/Costa_Rica"
readonly PRIVACY_TIME="20:00:00"
readonly PRIVACY_OFFSET="-06:00"

if [[ "${1:-}" == "commit" ]]; then
    current_date="$(TZ="${PRIVACY_TIMEZONE}" date +%Y-%m-%d)"
    privacy_timestamp="${current_date}T${PRIVACY_TIME}${PRIVACY_OFFSET}"

    export GIT_AUTHOR_DATE="${privacy_timestamp}"
    export GIT_COMMITTER_DATE="${privacy_timestamp}"
fi

exec "${REAL_GIT}" "$@"
EOF

chmod +x "${GIT_WRAPPER}"

echo "Git privacy wrapper installed."
echo "All new commits will use the current Costa Rica date at 20:00."