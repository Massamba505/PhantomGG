FROM node:20-alpine AS build

WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

FROM nginx:alpine

RUN apk add --no-cache gettext

COPY --from=build /app/dist/Client/browser /usr/share/nginx/html

COPY --from=build /app/src/env-config.template.js /usr/share/nginx/html/

COPY nginx.conf /etc/nginx/conf.d/default.conf

# Create entrypoint script with proper Unix line endings
RUN printf '#!/bin/sh\n\
# Generate env-config.js from template with environment variables\n\
envsubst < /usr/share/nginx/html/env-config.template.js > /usr/share/nginx/html/env-config.js\n\
\n\
# Start nginx\n\
exec "$@"\n' > /docker-entrypoint.sh && \
    chmod +x /docker-entrypoint.sh

EXPOSE 80

ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"]