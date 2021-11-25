import Home from "../views/Home.vue";

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home,
    meta: {
      icon: ["fas", "home"],
      renderRoute: false,
    },
  },
  {
    path: "/locations",
    name: "Locations",
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for ../views/Jobs/Jobs.vue
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ "@/views/Locations.vue"),
    meta: {
      icon: ["fas", "plus-square"],
      renderRoute: true,
    },
  },
  {
    path: "/files",
    name: "Files",
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for ../views/Jobs/Jobs.vue
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ "../views/Files.vue"),
    meta: {
      icon: ["fas", "briefcase"],
      renderRoute: true,
    },
  },
];

export default routes;
