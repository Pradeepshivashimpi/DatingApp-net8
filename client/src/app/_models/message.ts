export interface Message {
    id: number
    senderId: number
    senderUsername: string
    senderPhotoUrl: string
    receipientId: number
    receipientUsername: string
    receipientPhotoUrl: string
    content: string
    dateRead?: Date
    messageSent: Date
  }
  