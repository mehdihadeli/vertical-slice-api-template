#!/usr/bin/env bash

set -eax

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Run the setup_fonts.sh script from this directory regardless of the workspace cwd.
"$script_dir/setup-fonts.sh"
