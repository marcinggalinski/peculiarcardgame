<template>
  <header>Sign In</header>
  {{ returnUrl }}
  <div id="sign-in-form">
    <div>Username <InputText v-model="username" /></div>
    <div>Password <InputText type="password" v-model="password" /></div>
    <Button label="Sign in" @click="signIn()" />
  </div>
</template>

<script setup lang="ts">
import { inject } from "vue";

import { jwtDecode } from "jwt-decode";
import Button from "primevue/button";
import InputText from "primevue/inputtext";

import { UsersApiServiceKey } from "@/keys";
import UsersApiService from "@/services/users/apiService";
import { useUserStore } from "@/stores/user";
import { useRouter } from "vue-router";

const { returnUrl } = defineProps({
  returnUrl: {
    type: String,
    default: "/",
  },
});

const usersApiService = inject<UsersApiService>(UsersApiServiceKey);
if (!usersApiService) {
  throw new Error("UsersApiService not initialized");
}

const router = useRouter();
const userStore = useUserStore();

let username = "";
let password = "";

const signIn = async () => {
  const token = (await usersApiService.signIn(username, password)).token;
  const decodedToken = jwtDecode<{ id: number; name: string; nickname: string }>(token);

  userStore.signIn(
    {
      id: decodedToken.id,
      username: decodedToken.name,
      displayedName: decodedToken.nickname,
    },
    token
  );

  router.push(returnUrl);
};
</script>

<style scoped lang="stylus"></style>
