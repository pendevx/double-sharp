FROM node:20.10.0 as build

WORKDIR /webapp

COPY ./package.json ./package.json
COPY ./package-lock.json ./package-lock.json

RUN npm install
COPY . .
RUN npm run build


FROM nginx
COPY ./nginx/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /webapp/dist /site
