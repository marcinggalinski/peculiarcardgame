<template>
  <Dialog
    modal
    v-model:visible="visible"
    :draggable="false"
    :dismissable-mask="true"
    :close-on-escape="true"
    position="topright"
  >
    <template #header>
      <TabView v-model:active-index="activeIndex" class="full-width">
        <TabPanel header="Sign in" />
        <TabPanel header="Sign up" />
      </TabView>
    </template>

    <TabView v-model:active-index="activeIndex">
      <TabPanel header="Sign in" header-style="{ display: none; }">
        <div id="sign-in-form-container">
          <div class="field">
            <div>Username</div>
            <InputText v-model="signinFields.username" :disabled="isDisabled" />
          </div>
          <div class="field">
            <div>Password</div>
            <InputText type="password" v-model="signinFields.password" :disabled="isDisabled" />
          </div>
        </div>
      </TabPanel>
      <TabPanel header="Sign up">
        <div id="sign-up-form-container">
          <div class="field">
            <div>Username</div>
            <InputText v-model="signupFields.username" :disabled="isDisabled" />
          </div>
          <div class="field">
            <div>Displayed name</div>
            <InputText v-model="signupFields.displayedName" :disabled="isDisabled" />
          </div>
          <div class="field">
            <div>Password</div>
            <InputText type="password" v-model="signupFields.password" :disabled="isDisabled" />
          </div>
        </div>
      </TabPanel>
    </TabView>

    <template #footer>
      <Button
        v-if="activeIndex == Step.SignIn"
        class="float-right"
        label="Sign in!"
        @click="signIn()"
        :disabled="isDisabled"
      />
      <Button
        v-if="activeIndex == Step.SignUp"
        class="float-right"
        label="Sign up!"
        @click="signUp()"
        :disabled="isDisabled"
      />
    </template>
  </Dialog>
</template>
<script setup lang="ts">
import { inject, ref, watch } from "vue";

import Button from "primevue/button";
import Dialog from "primevue/dialog";
import InputText from "primevue/inputtext";
import TabView from "primevue/tabview";
import TabPanel from "primevue/tabpanel";
import { useToast } from "primevue/usetoast";

import { AxiosError } from "axios";

import { UsersServiceKey } from "@/keys";
import UsersService from "@/services/users/UsersService";

enum Step {
  SignIn = 0,
  SignUp = 1,
}

const visible = defineModel<boolean>("visible", { required: true });

const usersService = inject<UsersService>(UsersServiceKey);
if (!usersService) {
  throw new Error("UsersService is not initialized");
}

const toast = useToast();

const activeIndex = ref(0);
const signinFields = ref<{
  username: string;
  password: string;
}>({ username: "", password: "" });
const signupFields = ref<{
  username: string;
  displayedName: string;
  password: string;
}>({ username: "", displayedName: "", password: "" });
const isDisabled = ref(false);

watch(activeIndex, () => {
  switch (activeIndex.value) {
    case Step.SignIn:
      signupFields.value = {
        username: "",
        displayedName: "",
        password: "",
      };
      break;
    case Step.SignUp:
      signinFields.value = {
        username: "",
        password: "",
      };
      break;
  }
});

watch(visible, () => {
  if (!visible.value) {
    activeIndex.value = Step.SignIn;
    signinFields.value = { username: "", password: "" };
    signupFields.value = { username: "", displayedName: "", password: "" };
  }
});

const signIn = async () => {
  isDisabled.value = true;

  usersService.signIn(
    signinFields.value.username,
    signinFields.value.password,
    decodedToken => {
      visible.value = false;
      toast.add({
        summary: "Success",
        detail: `You are now signed in as ${decodedToken.name}.`,
        severity: "success",
        life: 3000,
      });
    },
    error => {
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
  );

  isDisabled.value = false;
};

const signUp = async () => {
  isDisabled.value = true;

  usersService.signUp(
    signupFields.value.username,
    signupFields.value.password,
    signupFields.value.displayedName,
    decodedToken => {
      visible.value = false;
      toast.add({
        summary: "Success",
        detail: `You are now signed in as ${decodedToken.name}.`,
        severity: "success",
        life: 3000,
      });
    },
    error => {
      if (error instanceof AxiosError) {
        const axiosError = error as AxiosError;
        if (axiosError.response?.status == 409) {
          toast.add({
            summary: "Sign up failed",
            detail: "Requested username is already in use. Please use a different one.",
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
  );

  isDisabled.value = false;
};
</script>
<style lang="stylus">
.p-dialog-footer
  align-items center

.float-left
  margin-right auto

.float-right
  margin-left auto

.full-width
  width 100%

.display-none
  display none

.p-dialog-header .p-tabview-panels
  display none

.p-dialog-content
  .p-tabview-nav-container
    display none

  .p-tabview-panels
    padding 0
</style>
