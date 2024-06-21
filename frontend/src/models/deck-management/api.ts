export enum CardType {
  Black = "Black",
  White = "White",
}

export interface AddCardRequest {
  cardType: CardType;
  text: string;
}
export interface AddDeckRequest {
  description?: string;
  name: string;
}
export interface GetCardResponse {
  cardType: CardType;
  id: number;
  text: string;
}
export interface GetDeckResponse {
  author: string;
  authorId: number;
  blackCardCount: number;
  description: string;
  id: number;
  name: string;
  whiteCardCount: number;
}
export interface UpdateCardRequest {
  textUpdate?: string;
}
export interface UpdateDeckRequest {
  descriptionUpdate?: string;
  nameUpdate?: string;
}
