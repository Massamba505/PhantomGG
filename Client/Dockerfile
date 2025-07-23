FROM node:20-alpine

WORKDIR /app/client

COPY package*.json ./
RUN npm install

COPY . .

EXPOSE 4200

CMD ["npm", "start"]