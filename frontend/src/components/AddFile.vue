<template>
  <v-card width="600px">
    <v-card-title class="text-h5">
      Upload File
    </v-card-title>
    <v-card-text>
      <v-row>
        <v-list subheader two-line flat>
          <v-subheader>Select lcoations</v-subheader>

          <v-list-item-group v-model="selectedLocations" multiple>
            <v-list-item v-for="(item, index) in locations" :key="index">
              <template v-slot:default="{ active }">
                <v-list-item-action>
                  <v-checkbox
                    :input-value="active"
                    color="primary"
                  ></v-checkbox>
                </v-list-item-action>

                <v-list-item-content>
                  <v-list-item-title>{{ item.path }}</v-list-item-title>
                  <v-list-item-subtitle>Id: {{ item.id }}</v-list-item-subtitle>
                </v-list-item-content>
              </template>
            </v-list-item>
          </v-list-item-group>
        </v-list>
      </v-row>
      <v-row class="mt-5">
        <p>
          Select a file to upload.
        </p>
        <v-file-input
          show-size
          label="File input"
          v-model="newFile"
          @change="selectFile"
        />
        <p v-if="fileName != null && fileName != ''">
          Selected File: {{ fileName }}
        </p>
      </v-row>
    </v-card-text>
    <v-card-actions>
      <v-spacer></v-spacer>
      <v-btn color="blue darken-1" text @click="submitFile">
        Upload
      </v-btn>
      <v-btn color="red darken-1" text @click="$emit('cancelFileUpload')">
        Cancel
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
export default {
  name: "AddFile",
  data() {
    return {
      newFile: undefined,
      fileName: "",
      locations: [],
      selectedLocations: []
    };
  },
  methods: {
    async loadLocations() {
      console.log("sdfsd");
      await this.$api
        .get("/location")
        .then(res => {
          this.locations = res.data;
        })
        .catch(err => {
          console.log(err);
        });
    },
    selectFile() {
      if (!this.newFile) {
        return;
      }
      this.message = "";
      this.fileName = this.newFile.name;
      this.newFileUploaded = true;
    },
    async submitFile() {
      console.log("sdfsdfssdf");
      this.message = "";
      const formData = new FormData();
      formData.append("file", this.newFile);
      formData.append("locations", this.getLocationsFromSelected());
      await this.$api
        .post("/file", formData, {
          timeout: 60 * 1000, //1 minute,
          headers: {
            "Content-Type": "multipart/form-data"
          }
        })
        .then(() => {
          this.message = "";
          this.uploadStep = false;
          this.newFile = null;
          this.newFileUploaded = false;
          this.processFileContents();
        })
        .catch(err => {
          this.message = `Could not upload the file. ${err.response.data.message}`;
          this.newFile = undefined;
        });
    },
    getLocationsFromSelected() {
      var locationsTmp = [];
      this.selectedLocations.forEach(item =>
        locationsTmp.push(this.locations[item].id)
      );

      return locationsTmp.join(",");
    }
  },
  created() {
    console.log("qwqw");
    this.loadLocations();
  }
};
</script>