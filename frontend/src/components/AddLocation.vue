<template>
  <v-card width="600px">
    <v-card-title class="text-h5">
      Add Location
    </v-card-title>
    <v-card-text>
      <v-row align="center" dense>
        <v-col cols="4">
          Folder Path
        </v-col>
        <v-col cols="8">
          <v-text-field v-model="location.path" label=""></v-text-field>
        </v-col>
      </v-row>
    </v-card-text>
    <v-card-actions>
      <v-spacer></v-spacer>
      <v-btn color="blue darken-1" text @click="submitLocation">
        Upload
      </v-btn>
      <v-btn color="red darken-1" text @click="$emit('close-add-location')">
        Cancel
      </v-btn>
    </v-card-actions>
    <v-alert v-if="message" border="left" color="blue-grey" dark>
      {{ message }}
    </v-alert>
  </v-card>
</template>

<script>
export default {
  name: "AddLocation",
  data() {
    return {
      location: {},
      message: ""
    };
  },
  methods: {
    async submitLocation() {
      await this.$api
        .post("/location", this.location)
        .then(() => {
          this.$emit("close-add-location");
        })
        .catch(err => {
          this.message = `Could not add location. ${err.response.data.detail}`;
        });
    }
  }
};
</script>