<template>
  <header>
    <div id="navbar-subtitle-logo" class="navbar-item">
      <RouterLink v-if="props.subtitle && props.subtitleHref" :to="props.subtitleHref" class="clear">
        <h1>{{ props.subtitle }}</h1>
      </RouterLink>
      <h1 v-else-if="props.subtitle">{{ props.subtitle }}</h1>
    </div>

    <div id="navbar-title-logo" class="navbar-item">
      <RouterLink v-if="props.titleHref" :to="props.titleHref" class="clear">
        <h1>{{ props.title }}</h1>
      </RouterLink>
      <h1 v-else>{{ props.title }}</h1>
    </div>

    <div id="navbar-user" class="navbar-item">
      <h1>
        <div v-if="userStore.isSignedIn">
          Signed in as {{ userStore.username }} | <a id="sign-out" @click="signOut()">Sign out</a>
        </div>
        <div v-else>
          <a id="sign-in" @click="signIn()">Sign in</a>
        </div>
      </h1>
    </div>
  </header>

  <SignIn v-model:visible="isSignInDialogVisible" />
</template>
<script setup lang="ts">
import { ref } from "vue";
import { RouterLink } from "vue-router";

import { useToast } from "primevue/usetoast";

import SignIn from "@/components/SignIn.vue";
import { useUserStore } from "@/stores/user";

const props = defineProps<{
  title: string;
  titleHref?: string;
  subtitle?: string;
  subtitleHref?: string;
}>();

const toast = useToast();
const userStore = useUserStore();

const isSignInDialogVisible = ref(false);

const signIn = () => {
  isSignInDialogVisible.value = true;
};

const signOut = () => {
  userStore.signOut();
  toast.add({
    summary: "Success",
    detail: "You are now signed out.",
    severity: "success",
    life: 3000,
  });
};
</script>
<style scoped lang="stylus">
header
  display flex
  justify-content center
  background-color black
  color white
  padding 10px

  .navbar-item
    flex 1
    margin 0

  a, h1
    margin 0
    display inline-block

  #navbar-title-logo
    text-align center

  #navbar-user
    text-align right

#sign-in
  cursor pointer

#sign-out
  cursor pointer
</style>
