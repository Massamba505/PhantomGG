#!/bin/sh

# Generate env-config.js from template with environment variables
envsubst < /usr/share/nginx/html/env-config.template.js > /usr/share/nginx/html/env-config.js

# Start nginx
exec "$@"
