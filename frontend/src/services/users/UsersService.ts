import { jwtDecode } from "jwt-decode";

import type DeckManagementApiService from "@/services/deck-management/apiService";
import type UsersApiService from "@/services/users/apiService";
import { useUserStore } from "@/stores/user";

export default class UsersService {
  private userStore;

  constructor(
    private usersApiService: UsersApiService,
    private deckManagementApiService: DeckManagementApiService
  ) {
    this.userStore = useUserStore();
  }

  // returns bearer token
  async signIn(
    username: string,
    password: string,
    successCallback?: (decodedToken: { id: string; name: string; nickname: string }) => void,
    failureCallback?: (error: unknown) => void
  ): Promise<void> {
    try {
      const token = (await this.usersApiService.signIn(username, password)).token;
      const decodedToken = jwtDecode<{ id: string; name: string; nickname: string }>(token);

      this.userStore.signIn(
        {
          id: Number(decodedToken.id),
          username: decodedToken.name,
          displayedName: decodedToken.nickname,
        },
        token
      );

      this.deckManagementApiService.setBearerToken(token);
      this.usersApiService.setBearerToken(token);

      if (successCallback) {
        successCallback(decodedToken);
      }
    } catch (error: unknown) {
      if (failureCallback) {
        failureCallback(error);
      }
    }
  }

  async signUp(
    username: string,
    password: string,
    displayedName?: string,
    successCallback?: (decodedToken: { id: string; name: string; nickname: string }) => void,
    failureCallback?: (error: unknown) => void
  ): Promise<void> {
    try {
      await this.usersApiService.addUser(username, password, displayedName);
      const token = (await this.usersApiService.signIn(username, password)).token;
      const decodedToken = jwtDecode<{ id: string; name: string; nickname: string }>(token);

      this.userStore.signIn(
        {
          id: Number(decodedToken.id),
          username: decodedToken.name,
          displayedName: decodedToken.nickname,
        },
        token
      );

      this.deckManagementApiService.setBearerToken(token);
      this.usersApiService.setBearerToken(token);

      if (successCallback) {
        successCallback(decodedToken);
      }
    } catch (error: unknown) {
      if (failureCallback) {
        failureCallback(error);
      }
    }
  }

  signOut(): void {
    this.usersApiService.unsetBearerToken();
    this.deckManagementApiService.unsetBearerToken();
    this.userStore.signOut();
  }
}
