import { Kitchen } from './Kitchen';

export class KitchenUser {
  kitchenUserId: number;
  kitchenId: number;
  userId: number;
  isOwner: boolean;
  hasAcceptedInvite: boolean;
  dateAdded: Date;
  username: string;
  kitchenName: string;
  kitchen: Kitchen;
}

