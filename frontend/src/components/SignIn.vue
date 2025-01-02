<template>
  <Dialog modal v-model:visible="visible">
    <div>Username <InputText v-model="username" /></div>
    <div>Password <InputText type="password" v-model="password" /></div>

    <template #footer>
      <Button
        class="float-left"
        severity="secondary"
        label="Cancel"
        @click="
          () => {
            visible = false;
          }
        "
      />
      <Button class="float-right" label="Save" @click="signIn()" />
    </template>
  </Dialog>
</template>
<script setup lang="ts">
import { inject, ref } from "vue";

import { AxiosError } from "axios";
import { jwtDecode } from "jwt-decode";
import Button from "primevue/button";
import Dialog from "primevue/dialog";
import InputText from "primevue/inputtext";
import { useToast } from "primevue/usetoast";

import { DeckManagementApiServiceKey, UsersApiServiceKey } from "@/keys";
import DeckManagementApiService from "@/services/deck-management/apiService";
import UsersApiService from "@/services/users/apiService";
import { useUserStore } from "@/stores/user";

const visible = defineModel<boolean>("visible", { required: true });

const deckManagementApiService = inject<DeckManagementApiService>(DeckManagementApiServiceKey);
if (!deckManagementApiService) {
  throw new Error("DeckManagementApiService not initialized");
}

const usersApiService = inject<UsersApiService>(UsersApiServiceKey);
if (!usersApiService) {
  throw new Error("UsersApiService not initialized");
}

const toast = useToast();
const userStore = useUserStore();

const username = ref("");
const password = ref("");

const signIn = async () => {
  try {
    const token = (await usersApiService.signIn(username.value, password.value)).token;
    const decodedToken = jwtDecode<{ id: string; name: string; nickname: string }>(token);

    userStore.signIn(
      {
        id: Number(decodedToken.id),
        username: decodedToken.name,
        displayedName: decodedToken.nickname,
      },
      token
    );

    deckManagementApiService.setBearerToken(token);
    usersApiService.setBearerToken(token);

    visible.value = false;
    toast.add({
      summary: "Success",
      detail: `You are now signed in as ${decodedToken.name}.`,
      severity: "success",
      life: 3000,
    });
  } catch (error: unknown) {
    if (error instanceof AxiosError) {
      const axiosError = error as AxiosError;
      if (axiosError.response?.status == 401) {
        toast.add({
          summary: "Sign in failed",
          detail: "Make sure you entered correct username and password and try again.",
          severity: "error",
          life: 3000,
        });
      }
    } else {
      console.log(error);
      toast.add({
        summary: "Unknown error",
        detail: "Try again later or contact the support if the issue persists.",
        severity: "error",
        life: 3000,
      });
    }
  }
};
</script>
<style lang="stylus"></style>
