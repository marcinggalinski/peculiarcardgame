import { jwtDecode } from "jwt-decode";

import type DeckManagementApiService from "@/services/deck-management/apiService";
import type UsersApiService from "@/services/users/apiService";
import type AuthApiService from "@/services/auth/apiService";
import { useUserStore } from "@/stores/user";
import { useToast } from "primevue/usetoast";

const localStorageKeys = {
  accessToken: "jwt",
  refreshToken: "refresh",
};

export default class UsersService {
  private userStore = useUserStore();
  private toast = useToast();
  private refreshTokensTimeoutIdentifier: number = 0;

  constructor(
    private authApiService: AuthApiService,
    private usersApiService: UsersApiService,
    private deckManagementApiService: DeckManagementApiService
  ) {
    this.refreshTokens(false);
  }

  async refreshTokens(notifySignOut: boolean = true): Promise<void> {
    const refreshToken = localStorage.getItem(localStorageKeys.refreshToken);
    if (!refreshToken) return;

    const tokens = await this.authApiService.refreshAccessToken(refreshToken);
    if (!tokens) {
      if (notifySignOut)
        this.toast.add({
          summary: "Signed out",
          detail: "You have been signed out",
          severity: "warn",
          life: 3000,
        });

      this.clearTokens();
      if (this.refreshTokensTimeoutIdentifier) clearTimeout(this.refreshTokensTimeoutIdentifier);
      return;
    }

    const decoded = this.decodeJwt(tokens.accessToken);

    localStorage.setItem(localStorageKeys.accessToken, tokens.accessToken);
    localStorage.setItem(localStorageKeys.refreshToken, tokens.refreshToken);
    this.deckManagementApiService.setBearerToken(tokens.accessToken);
    this.usersApiService.setBearerToken(tokens.accessToken);
    this.userStore.signIn({ id: Number(decoded.id), username: decoded.name, displayedName: decoded.nickname });

    this.refreshTokensTimeoutIdentifier = setTimeout(() => {
      this.refreshTokens();
    }, decoded.ttlMilliseconds - 60000);
  }

  async signIn(
    username: string,
    password: string,
    successCallback?: (decodedToken: { id: string; name: string; nickname: string }) => void,
    failureCallback?: (error: unknown) => void
  ): Promise<void> {
    try {
      const tokens = await this.authApiService.signIn(username, password);
      const decodedToken = jwtDecode<{ id: string; name: string; nickname: string }>(tokens.accessToken);

      localStorage.setItem(localStorageKeys.accessToken, tokens.accessToken);
      localStorage.setItem(localStorageKeys.refreshToken, tokens.refreshToken);

      this.deckManagementApiService.setBearerToken(tokens.accessToken);
      this.usersApiService.setBearerToken(tokens.accessToken);
      this.userStore.signIn({
        id: Number(decodedToken.id),
        username: decodedToken.name,
        displayedName: decodedToken.nickname,
      });

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
    await this.usersApiService.addUser(username, password, displayedName);
    await this.signIn(username, password, successCallback, failureCallback);
  }

  async signOut(): Promise<void> {
    const refreshToken = localStorage.getItem(localStorageKeys.refreshToken)!;

    await this.authApiService.revokeRefreshToken(refreshToken);
    this.clearTokens();
  }

  clearTokens(): void {
    this.usersApiService.unsetBearerToken();
    this.deckManagementApiService.unsetBearerToken();
    this.userStore.signOut();

    localStorage.removeItem(localStorageKeys.accessToken);
    localStorage.removeItem(localStorageKeys.refreshToken);
  }

  private decodeJwt(jwt: string): { id: string; name: string; nickname: string; ttlMilliseconds: number } {
    try {
      const decoded = jwtDecode<{ id: string; name: string; nickname: string; exp: number }>(jwt);
      const ttl = decoded.exp * 1000 - Date.now();
      if (ttl < 0) {
        return { id: "", name: "", nickname: "", ttlMilliseconds: 0 };
      }
      return { id: decoded.id, name: decoded.name, nickname: decoded.nickname, ttlMilliseconds: ttl };
    } catch (error: unknown) {
      console.error("Error while decoding JWT:");
      console.error(error);
      return { id: "", name: "", nickname: "", ttlMilliseconds: 0 };
    }
  }
}
