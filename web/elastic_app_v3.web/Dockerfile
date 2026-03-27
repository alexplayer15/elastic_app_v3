FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*

COPY dist /usr/share/nginx/html

COPY identityStyle.css /usr/share/nginx/html

COPY web_pages /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]