<template>
  <Toast />
  <Suspense>
    <RouterView />
  </Suspense>
</template>

<script setup lang="ts">
import Toast from "primevue/toast";
import { provide } from "vue";
import { RouterView } from "vue-router";
import { DeckManagementApiServiceKey, UsersApiServiceKey } from "@/keys";
import DeckManagementApiService from "@/services/deck-management/apiService";
import UsersApiService from "@/services/users/apiService";

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL as string;

provide(DeckManagementApiServiceKey, new DeckManagementApiService(apiBaseUrl));
provide(UsersApiServiceKey, new UsersApiService(apiBaseUrl));
</script>

<style lang="stylus">
.clear
  text-decoration unset
  color unset
</style>
