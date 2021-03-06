<template>
  <div>
    <v-row align="center">
      <v-col cols="auto">
        <!--class="mr-auto"-->
        <h1>Files</h1>
      </v-col>
      <v-col cols="auto">
        <v-btn
          icon
          small
          class="mx-2"
          elevation="2"
          @click="addFileDialog = true"
        >
          <font-awesome-icon :icon="['fas', 'plus']" />
        </v-btn>
      </v-col>
    </v-row>
    <v-data-table
      class="elevation-1"
      :headers="headers"
      :items="files"
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
      <template v-slot:[`item.timestamp`]="{ item }">
        {{ getDateTextField(item.timestamp) }}
      </template>
      <template v-slot:[`item.actions`]="{ item }">
        <v-btn
          icon
          small
          class="mx-2 mb-2"
          elevation="2"
          @click.stop="deleteFile(item.id)"
        >
          <font-awesome-icon :icon="['fas', 'trash-alt']" />
        </v-btn>
        <v-btn
          icon
          small
          class="mx-2 mb-2"
          elevation="2"
          @click.stop="downloadFile(item.id)"
        >
          <font-awesome-icon :icon="['fas', 'download']" />
        </v-btn>
      </template>
    </v-data-table>

    <LoadingDialog v-model="loadingFileDialog" :text="loadingFileDialogText">
    </LoadingDialog>

    <v-dialog v-model="addFileDialog" persistent width="unset">
      <!--The v-if on the component makes sure that the component is killed once the dialog closes, so all data is cleared-->
      <AddFile v-if="addFileDialog" @close-add-file="fileDialogClosed">
      </AddFile>
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
import moment from "moment";
import LoadingDialog from "./LoadingDialog.vue";
import AddFile from "./AddFile.vue";

export default {
  name: "FileList",
  components: {
    LoadingDialog,
    AddFile
  },
  data: () => ({
    headers: [
      {
        text: "Id",
        align: "start",
        sortable: false,
        value: "id"
      },
      { text: "Name", value: "name" },
      { text: "Type", value: "type" },
      { text: "Timestamp", value: "timestamp" },
      { text: "Actions", value: "actions", sortable: false }
    ],
    expanded: [],
    isDataLoading: true,
    files: [],
    loadingFileDialog: false,
    loadingFileDialogText: "",
    snackbar: false,
    snackbarText: "",
    snackbarTimeout: 3000,
    addFileDialog: false
  }),
  methods: {
    getDateTextField(date) {
      return date ? moment(date).format("dddd, MMMM Do YYYY") : "";
    },
    async loadFiles() {
      this.isDataLoading = true;
      await this.$api
        .get("/file")
        .then(res => {
          this.files = res.data;
          this.isDataLoading = false;
        })
        .catch(err => {
          this.invalidConnection(err.response.data.detail);
        });
    },
    async deleteFile(id) {
      await this.$api
        .delete(`/file/${id}`)
        .then(() => {
          this.loadFiles();
        })
        .catch(err => {
          this.invalidConnection(err.response.data.detail);
        });
    },
    async downloadFile(id) {
      this.loadingFileDialog = true;
      this.loadingFileDialogText = "Downloading file.";

      await this.$api
        .get(`/file/${id}`, {
          responseType: "blob",
          timeout: 600000, //10 minutes
          headers: {
            "Content-Type": "application/json"
          }
        })
        .then(response => {
          this.loadingFileDialog = false;
          this.loadingFileDialogText = "";

          this.createSnackbar("File downloaded successfully.");
          var filename = "";
          var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
          var matches = filenameRegex.exec(
            response.headers["content-disposition"]
          );

          if (matches != null && matches[1]) {
            filename = matches[1].replace(/['"]/g, "");
          }

          const url = window.URL.createObjectURL(new Blob([response.data]));
          const link = document.createElement("a");
          link.href = url;
          link.setAttribute("download", filename); //or any other extension
          document.body.appendChild(link);
          link.click();
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
    fileDialogClosed() {
      console.log("fileDialogClosed");
      this.addfileDialog = false;
      this.loadFiles();
    }
  },
  created() {
    this.loadFiles();
  }
};
</script>

<style scoped>
</style>