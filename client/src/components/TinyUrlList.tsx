import { useEffect, useState } from "react";
import { apiClient } from "../lib/apiClient";
import type { TinyUrlRecord } from "../types";

interface TinyUrlListProps {
  refreshTrigger: number
}

export default function TinyUrlList({ refreshTrigger }: TinyUrlListProps) {
  const [tinyUrls, setTinyUrls] = useState<TinyUrlRecord[]>([]);

  const fetchUrls = async () => {
    try {
      const data = await apiClient.listTinyUrls();
      setTinyUrls(data);
    } catch (error) {
      console.error("Error fetching URLs:", error);
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await apiClient.deleteTinyUrl(id);
      fetchUrls();
    } catch (error) {
      console.error("Error deleting URL:", error);
    }
  };

  useEffect(() => {
    fetchUrls();
  }, [refreshTrigger]);

  return (
    <div>
      <h2>Tiny URL List</h2>
      <ul className="list-disc list-inside">
        {tinyUrls.map((url) => (
          <li key={url.id}>
            ID: {url.id} | Short URL: {url.shortUrl} | Long URL: {url.longUrl} | <button onClick={() => handleDelete(url.id)} className="text-red-500 hover:text-red-600 cursor-pointer">Delete</button>
          </li>
        ))}
      </ul>
    </div>
  );
}
