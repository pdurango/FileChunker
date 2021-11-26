<template>
  <v-row justify="center">
    <v-dialog v-model="value" persistent width="350">
      <v-card color="primary" dark>
        <v-card-text>
          {{ text }}
          <v-progress-linear
            indeterminate
            color="white"
            class="mb-0"
          ></v-progress-linear>
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-row>
</template>



<script>
export default {
  name: "LoadingDialog",
  props: ["value", "text"],
  data() {
    return {};
  }
};
</script>