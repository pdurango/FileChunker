import Vue from "vue";
import App from "./App.vue";
import vuetify from "./plugins/vuetify";
import router from "./router";
import "./plugins/fontawesome";
import api from "./services/api";

Vue.config.productionTip = false;
Vue.prototype.$api = api;
api.defaults.timeout = 10000;

// api.interceptors.request.use((request) => {
//   console.log("Starting Request", JSON.stringify(request, null, 2));
//   return request;
// });

// api.interceptors.response.use((response) => {
//   console.log("Response:", JSON.stringify(response, null, 2));
//   return response;
// });

new Vue({
  vuetify,
  router,
  render: (h) => h(App),
}).$mount("#app");
