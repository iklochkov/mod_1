using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCRM.Application.DTOs
{
    public class ClientAvatarDto
    {
    public int ClientId { get; set; }
    public string AvatarUrl { get; set; }
    public long FileSize { get; set; }
    public string FileName { get; set; }

    public ClientAvatarDto(int clientId, string avatarUrl, long fileSize, string fileName)
    {
        ClientId = clientId;
        AvatarUrl = avatarUrl;
        FileSize = fileSize;
        FileName = fileName;
    }

}
}