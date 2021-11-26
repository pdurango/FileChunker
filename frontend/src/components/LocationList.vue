<template>
  <div>
    <v-row align="center">
      <v-col cols="auto">
        <!--class="mr-auto"-->
        <h1>Locations</h1>
      </v-col>
      <!-- <v-col cols="auto">
        <v-btn
          icon
          small
          class="mx-2"
          elevation="2"
          @click="
            $router.push({
              name: 'JobInfoManagementNew'
            })
          "
        >
          <font-awesome-icon :icon="['fas', 'plus']" />
        </v-btn>
      </v-col> -->
    </v-row>
    <v-data-table
      class="elevation-1"
      :headers="headers"
      :items="locations"
      sort-by="id"
      single-expand
      :expanded.sync="expanded"
      item-key="id"
      :loading="isDataLoading"
      loading-text="Loading... Please wait"
      dense
      fixed-header
      disable-sort
    >
      <template v-slot:[`item.actions`]="{ item }">
        <v-btn
          icon
          small
          class="mx-2 mb-2"
          elevation="2"
          @click.stop="deleteLocation(item.id)"
        >
          <font-awesome-icon :icon="['fas', 'trash-alt']" />
        </v-btn>
      </template>
    </v-data-table>
  </div>
</template>

<script>
export default {
  name: "LocationList",
  data: () => ({
    headers: [
      {
        text: "Id",
        align: "start",
        sortable: false,
        value: "id"
      },
      { text: "Path", value: "path" },
      { text: "Actions", value: "actions", sortable: false }
    ],
    expanded: [],
    isDataLoading: true,
    locations: []
  }),
  methods: {
    async loadLocations() {
      this.isDataLoading = true;
      await this.$api
        .get("/location")
        .then(res => {
          this.locations = res.data;
          this.isDataLoading = false;
        })
        .catch(err => {
          console.log(err);
          this.invalidConnection();
        });
    },
    async deleteLocation(id) {
      await this.$api
        .delete(`/location.${id}`)
        .then(() => {
          this.loadLocations();
        })
        .catch(err => {
          console.log(err);
          this.invalidConnection();
        });
    },
    invalidConnection() {
      this.jobs = [];
      this.snackbar = true;
      this.snackbarText = "Cannot get job information from server.";
      this.isDataLoading = true;
    }
  },
  created() {
    this.loadLocations();
  }
};
</script>

<style scoped>
</style>