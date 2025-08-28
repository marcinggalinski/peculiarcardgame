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
            <div>Username <small class="red">*</small></div>
            <InputText
              v-model="signin.fields.username"
              :disabled="isLoading"
              :invalid="signin.invalid"
              @change="() => (signin.invalid = false)"
              maxlength="30"
            />
          </div>
          <div class="field">
            <div>Password <small class="red">*</small></div>
            <InputText
              type="password"
              v-model="signin.fields.password"
              :disabled="isLoading"
              :invalid="signin.invalid"
              @change="() => (signin.invalid = false)"
            />
          </div>

          <div class="field">
            <small class="red">* Field is required</small>
          </div>
        </div>
      </TabPanel>
      <TabPanel header="Sign up">
        <div id="sign-up-form-container">
          <div class="field">
            <div>Username <small class="red">*</small></div>
            <InputText
              v-model="signup.fields.username"
              :disabled="isLoading"
              :invalid="signup.invalid.username"
              @change="() => (signup.invalid.username = false)"
              maxlength="30"
            />
          </div>
          <div class="field">
            <div>Displayed name</div>
            <InputText v-model="signup.fields.displayedName" :disabled="isLoading" maxlength="30" />
          </div>
          <div class="field">
            <div>Password <small class="red">*</small></div>
            <InputText
              type="password"
              v-model="signup.fields.password"
              :disabled="isLoading"
              :invalid="signup.invalid.password"
              @change="() => (signup.invalid.password = false)"
            />
          </div>

          <div class="field">
            <small class="red">* Field is required</small>
          </div>
        </div>
      </TabPanel>
    </TabView>

    <template #footer>
      <Button
        v-if="activeIndex == Step.SignIn"
        class="float-right"
        label="Sign in!"
        icon="pi pi-sign-in"
        @click="signIn()"
        :loading="isLoading"
        :disabled="isLoading || !signin.fields.username || !signin.fields.password"
      />
      <Button
        v-if="activeIndex == Step.SignUp"
        class="float-right"
        label="Sign up!"
        icon="pi pi-user-plus"
        @click="signUp()"
        :loading="isLoading"
        :disabled="isLoading || !signup.fields.username || !signup.fields.password"
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

import { UsersServiceKey } from "@/keys";
import UsersService from "@/services/users/usersService";
import ApiError from "@/models/apiError";

enum Step {
  SignIn = 0,
  SignUp = 1,
}

const signinDefault = () => {
  return {
    fields: {
      username: "",
      password: "",
    },
    invalid: false,
  };
};
const signupDefault = () => {
  return {
    fields: {
      username: "",
      displayedName: "",
      password: "",
    },
    invalid: {
      username: false,
      password: false,
    },
  };
};

const visible = defineModel<boolean>("visible", { required: true });

const usersService = inject<UsersService>(UsersServiceKey);
if (!usersService) {
  throw new Error("UsersService is not initialized");
}

const toast = useToast();

const activeIndex = ref(0);
const signin = ref<{
  fields: {
    username: string;
    password: string;
  };
  invalid: boolean;
}>(signinDefault());
const signup = ref<{
  fields: {
    username: string;
    displayedName: string;
    password: string;
  };
  invalid: {
    username: boolean;
    password: boolean;
  };
}>(signupDefault());
const isLoading = ref(false);

watch(activeIndex, () => {
  switch (activeIndex.value) {
    case Step.SignIn:
      signup.value = signupDefault();
      break;
    case Step.SignUp:
      signin.value = signinDefault();
      break;
  }
});

watch(visible, () => {
  if (!visible.value) {
    activeIndex.value = Step.SignIn;
    signin.value = signinDefault();
    signup.value = signupDefault();
  }
});

const signIn = async () => {
  isLoading.value = true;
  signin.value.invalid = false;

  await usersService.signIn(
    signin.value.fields.username.trim(),
    signin.value.fields.password,
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
      if (error instanceof ApiError) {
        const apiError = error as ApiError;
        if (apiError.statusCode == 401) {
          signin.value.invalid = true;
          toast.add({
            summary: "Sign in failed",
            detail: "Make sure you entered correct username and password and try again.",
            severity: "error",
            life: 3000,
          });
        }
      } else {
        console.error("Error signing in:");
        console.error(error);
        toast.add({
          summary: "Unknown error",
          detail: "Try again later or contact the support if the issue persists.",
          severity: "error",
          life: 3000,
        });
      }
    }
  );

  isLoading.value = false;
};

const signUp = async () => {
  isLoading.value = true;
  signup.value.invalid.username = false;
  signup.value.invalid.password = false;

  await usersService.signUp(
    signup.value.fields.username.trim(),
    signup.value.fields.password,
    signup.value.fields.displayedName.trim(),
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
      if (error instanceof ApiError) {
        const apiError = error as ApiError;
        if (apiError.statusCode == 409) {
          signup.value.invalid.username = true;
          toast.add({
            summary: "Sign up failed",
            detail: "Requested username is already in use. Please use a different one.",
            severity: "error",
            life: 3000,
          });
        }
      } else {
        console.error("Error signing up:");
        console.error(error);
        toast.add({
          summary: "Unknown error",
          detail: "Try again later or contact the support if the issue persists.",
          severity: "error",
          life: 3000,
        });
      }
    }
  );

  isLoading.value = false;
};
</script>
<style scoped lang="stylus">
.field
  margin-bottom 15px

.field:last-of-type
  margin-bottom unset
</style>
<style lang="stylus">
.p-dialog-header .p-tabview-panels
  display none

.p-dialog-content
  .p-tabview-nav-container
    display none

  .p-tabview-panels
    padding 0
</style>
