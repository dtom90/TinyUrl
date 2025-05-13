interface TinyUrlRequest {
  longUrl: string;
}

interface TinyUrlRecord {
  id: string;
  longUrl: string;
  shortUrl: string;
  clickCount: number;
}

export type { TinyUrlRequest, TinyUrlRecord };
