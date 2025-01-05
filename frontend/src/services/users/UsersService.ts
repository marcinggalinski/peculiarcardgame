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
    this.trySignInFromLocalStorage();
  }

  async trySignInFromLocalStorage() {
    const token = localStorage.getItem("jwt");
    if (!token) return;

    const decodedToken = this.decodeToken(token);
    if (!decodedToken.id || !decodedToken.name || !decodedToken.nickname) {
      localStorage.removeItem("jwt");
      return;
    }

    this.deckManagementApiService.setBearerToken(token);
    this.usersApiService.setBearerToken(token);
    this.userStore.signIn(
      {
        id: Number(decodedToken.id),
        username: decodedToken.name,
        displayedName: decodedToken.nickname,
      },
      token
    );
  }

  decodeToken(token: string): { id: string; name: string; nickname: string } {
    try {
      const decoded = jwtDecode<{ id: string; name: string; nickname: string; exp: number }>(token);
      if (Date.now() > decoded.exp * 1000) {
        return { id: "", name: "", nickname: "" };
      }
      return { id: decoded.id, name: decoded.name, nickname: decoded.nickname };
    } catch (error: unknown) {
      console.error("Error while decoding JWT:");
      console.error(error);
      return { id: "", name: "", nickname: "" };
    }
  }

  /**
   * @remarks
   * Returns bearer token
   */
  async signIn(
    username: string,
    password: string,
    successCallback?: (decodedToken: { id: string; name: string; nickname: string }) => void,
    failureCallback?: (error: unknown) => void
  ): Promise<void> {
    try {
      const token = (await this.usersApiService.signIn(username, password)).token;
      const decodedToken = jwtDecode<{ id: string; name: string; nickname: string }>(token);

      this.deckManagementApiService.setBearerToken(token);
      this.usersApiService.setBearerToken(token);
      this.userStore.signIn(
        {
          id: Number(decodedToken.id),
          username: decodedToken.name,
          displayedName: decodedToken.nickname,
        },
        token
      );

      localStorage.setItem("jwt", token);

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

      this.deckManagementApiService.setBearerToken(token);
      this.usersApiService.setBearerToken(token);
      this.userStore.signIn(
        {
          id: Number(decodedToken.id),
          username: decodedToken.name,
          displayedName: decodedToken.nickname,
        },
        token
      );

      localStorage.setItem("jwt", token);

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
    localStorage.removeItem("jwt");
  }
}
