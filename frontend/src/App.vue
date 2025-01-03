<template>
  <Toast />
  <Suspense>
    <RouterView />
  </Suspense>
</template>

<script setup lang="ts">
import { provide } from "vue";
import { RouterView } from "vue-router";

import Toast from "primevue/toast";

import { DeckManagementApiServiceKey, UsersApiServiceKey, UsersServiceKey } from "@/keys";
import DeckManagementApiService from "@/services/deck-management/apiService";
import UsersApiService from "@/services/users/apiService";
import UsersService from "@/services/users/UsersService";

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL as string;

const deckManagementApiService = new DeckManagementApiService(apiBaseUrl);
const usersApiService = new UsersApiService(apiBaseUrl);
const usersService = new UsersService(usersApiService, deckManagementApiService);

provide(DeckManagementApiServiceKey, deckManagementApiService);
provide(UsersApiServiceKey, usersApiService);
provide(UsersServiceKey, usersService);
</script>

<style lang="stylus">
.clear
  text-decoration unset
  color unset
</style>
