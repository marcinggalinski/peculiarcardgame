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

import { DeckManagementApiServiceKey, UsersServiceKey } from "@/keys";
import AuthApiService from "@/services/auth/apiService";
import DeckManagementApiService from "@/services/deck-management/apiService";
import UsersApiService from "@/services/users/apiService";
import UsersService from "@/services/users/usersService";

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL as string;

const authApiService = new AuthApiService(apiBaseUrl);
const deckManagementApiService = new DeckManagementApiService(apiBaseUrl);
const usersApiService = new UsersApiService(apiBaseUrl);
const usersService = new UsersService(authApiService, usersApiService, deckManagementApiService);

provide(DeckManagementApiServiceKey, deckManagementApiService);
provide(UsersServiceKey, usersService);
</script>

<style lang="stylus"></style>
