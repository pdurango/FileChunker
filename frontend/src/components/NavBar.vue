<template>
  <div>
    <!-- @transitionend="handleDrawer" -->
    <v-navigation-drawer v-model="drawer" app right stateless>
      <v-list>
        <v-list-item
          v-for="link in routes"
          :key="link.name"
          router
          :to="link.path"
        >
          <div
            v-if="link.name == 'Home'"
            style="display: flex; font-weight: bold"
          >
            <v-list-item-action>
              <!-- <font-awesome-icon :icon="link.meta.icon" /> -->
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>ChunkyBoi</v-list-item-title>
            </v-list-item-content>
          </div>
          <div v-else-if="link.meta.renderRoute" style="display: flex">
            <v-list-item-action>
              <!-- <font-awesome-icon :icon="link.meta.icon" /> -->
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>{{ link.name }}</v-list-item-title>
            </v-list-item-content>
          </div>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-toolbar color="#003E5F" shrink-on-scroll class="nav-bar">
      <span class="hidden-sm-and-up">
        <v-app-bar-nav-icon
          style="color: white"
          @click="drawer = !drawer"
        ></v-app-bar-nav-icon>
      </span>
      <v-toolbar-title
        style="cursor: pointer; color: white"
        @click="$router.push({ name: 'Home' }).catch(() => {})"
        >OneChunkBoi</v-toolbar-title
      >
      <v-spacer />
      <v-toolbar-items
        v-for="link in routes"
        :key="link.name"
        class="hidden-xs-only"
      >
        <v-btn
          plain
          text
          v-if="link.meta.renderRoute"
          router
          :to="link.path"
          color="white"
        >
          <font-awesome-icon class="mr-1" :icon="link.meta.icon" />
          {{ link.name }}
        </v-btn>
      </v-toolbar-items>
    </v-toolbar>
  </div>
</template>

<script>
import vars from "@/assets/global.scss";
import routes from "../router/routes";

export default {
  name: "NavBar",
  data() {
    return {
      drawer: false,
      routes,
      vars
    };
  },
  methods: {
    // eslint-disable-next-line no-unused-vars
    onResize(event) {
      this.handleDrawer();
    },
    handleDrawer() {
      if (window.innerWidth >= 576) {
        this.drawer = false;
      }
    }
  },
  mounted() {
    // Register an event listener when the Vue component is ready
    window.addEventListener("resize", this.onResize);
  },
  beforeDestroy() {
    // Unregister the event listener before destroying this Vue instance
    window.removeEventListener("resize", this.onResize);
  }
};
</script>

<style lang="scss" scoped>
@import "@/assets/global.scss";
.nav-bar {
  height: $app-bar-height !important;
}
</style>
