import { defineStore } from "pinia";
import { ref } from "vue";

export const useUserStore = defineStore("user", () => {
  const isSignedIn = ref(false);

  const id = ref<number>();
  const username = ref<string>();
  const displayedName = ref<string>();

  function anonymousSignIn(name: string) {
    displayedName.value = name;
  }

  function signIn(user: { id: number; username: string; displayedName: string }) {
    isSignedIn.value = true;
    id.value = user.id;
    username.value = user.username;
    displayedName.value = user.displayedName;
  }

  function signOut() {
    isSignedIn.value = false;
    id.value = undefined;
    username.value = undefined;
    displayedName.value = undefined;
  }

  return {
    isSignedIn,
    id,
    username,
    displayedName,
    anonymousSignIn,
    signIn,
    signOut,
  };
});
