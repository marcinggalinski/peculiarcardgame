import type { GetUserResponse } from "@/models/users/api";
import { defineStore } from "pinia";
import { ref } from "vue";

export const useUserStore = defineStore("user", () => {
  const isSignedIn = ref(false);

  const id = ref<number>();
  const username = ref<string>();
  const displayedName = ref<string>();

  const bearerToken = ref<string>();

  function anonymousSignIn(name: string) {
    displayedName.value = name;
  }

  function signIn(user: GetUserResponse, token: string) {
    isSignedIn.value = true;
    id.value = user.id;
    username.value = user.username;
    displayedName.value = user.displayedName;
    bearerToken.value = token;
  }

  function signOut() {
    isSignedIn.value = false;
    id.value = undefined;
    username.value = undefined;
    displayedName.value = undefined;
    bearerToken.value = undefined;
  }

  return {
    isSignedIn,
    id,
    username,
    displayedName,
    bearerToken,
    anonymousSignIn,
    signIn,
    signOut,
  };
});
