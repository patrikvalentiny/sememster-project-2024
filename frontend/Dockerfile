FROM node:21.7 as build

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

RUN npm run build -- --configuration=development

FROM nginx:latest

COPY --from=build app/dist/browser /usr/share/nginx/html

EXPOSE 80
