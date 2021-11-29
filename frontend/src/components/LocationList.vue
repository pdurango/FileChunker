<template>
  <div>
    <v-row align="center">
      <v-col cols="auto">
        <!--class="mr-auto"-->
        <h1>Locations</h1>
      </v-col>
      <v-col cols="auto">
        <v-btn
          icon
          small
          class="mx-2"
          elevation="2"
          @click="addLocationDialog = true"
        >
          <font-awesome-icon :icon="['fas', 'plus']" />
        </v-btn>
      </v-col>
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
    <v-dialog v-model="addLocationDialog" persistent width="unset">
      <!--The v-if on the component makes sure that the component is killed once the dialog closes, so all data is cleared-->
      <AddLocation
        v-if="addLocationDialog"
        @close-add-location="locationDialogClosed"
      >
      </AddLocation>
    </v-dialog>

    <v-snackbar v-model="snackbar" :timeout="snackbarTimeout">
      {{ snackbarText }}
      <template v-slot:action="{ attrs }">
        <v-btn color="blue" text v-bind="attrs" @click="snackbar = false">
          Close
        </v-btn>
      </template>
    </v-snackbar>
  </div>
</template>

<script>
import AddLocation from "./AddLocation.vue";

export default {
  name: "LocationList",
  components: {
    AddLocation
  },
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
    locations: [],
    addLocationDialog: false,
    snackbar: false,
    snackbarText: "",
    snackbarTimeout: 3000
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
          this.invalidConnection(err.response.data.detail);
        });
    },
    async deleteLocation(id) {
      await this.$api
        .delete(`/location/${id}`)
        .then(() => {
          this.createSnackbar(`Deleted location ${id}.`);
          this.loadLocations();
        })
        .catch(err => {
          this.invalidConnection(err.response.data.detail);
        });
    },
    invalidConnection(message) {
      this.createSnackbar(`Could not complete action. ${message}`);
    },
    createSnackbar(message) {
      this.snackbar = true;
      this.snackbarText = message;
    },
    locationDialogClosed() {
      this.addLocationDialog = false;
      this.loadLocations();
    }
  },
  created() {
    this.loadLocations();
  }
};
</script>

<style scoped>
</style>