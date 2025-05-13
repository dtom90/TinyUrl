interface TinyUrlRequest {
  longUrl: string;
}

interface TinyUrlRecord {
  id: string;
  longUrl: string;
  shortUrl: string;
}

export type { TinyUrlRequest, TinyUrlRecord };
