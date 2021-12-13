using System;
using Multiplay;

public class Network
{
    /// <summary>
    /// 启动服务器
    /// </summary>
    /// <param name="ip">IPv4地址</param>
    public Network(string ip)
    {
        //注册
        Server.Register(MessageType.HeartBeat, _HeartBeat);
        Server.Register(MessageType.Enroll, _Enroll);
        Server.Register(MessageType.CreatRoom, _CreatRoom);
        Server.Register(MessageType.EnterRoom, _EnterRoom);
        Server.Register(MessageType.ExitRoom, _ExitRoom);
        Server.Register(MessageType.StartGame, _StartGame);
        Server.Register(MessageType.ExitGame, _ExitGame);
        Server.Register(MessageType.Move, _PlayMove);
        Server.Register(MessageType.PlayVoice, _PlayVoice);
        //启动服务器
        Server.Start(ip);
    }

    private void _HeartBeat(Player player, byte[] data)
    {
        //仅做回应
        player.Send(MessageType.HeartBeat);
    }

    private void _Enroll(Player player, byte[] data)
    {
        Enroll result = new Enroll();

        Enroll receive = NetworkUtils.Deserialize<Enroll>(data);

        Console.WriteLine($"玩家{player.Name}改名为{receive.Name}");
        //设置玩家名字
        player.Name = receive.Name;

        //向玩家发送成功操作结果
        result.Suc = true;
        result.Name = receive.Name;
        data = NetworkUtils.Serialize(result);
        player.Send(MessageType.Enroll, data);
    }

    private void _CreatRoom(Player player, byte[] data)
    {
        //结果
        CreatRoom result = new CreatRoom();
        CreatRoom receive = NetworkUtils.Deserialize<CreatRoom>(data);
        Console.WriteLine(player.InRoom);
        Console.WriteLine(receive.Suc);
        //逻辑检测(玩家不在任何房间中 并且 不存在该房间)
        if (!player.InRoom && !Server.Rooms.ContainsKey(receive.RoomId))
        {
            //新增房间
            Room room = new Room(receive.RoomId);
            Server.Rooms.Add(room.RoomId, room);
            //添加玩家
            room.Players.Add(player);
            player.EnterRoom(receive.RoomId);

            Console.WriteLine($"玩家:{player.Name}创建房间成功");

            //向客户端发送操作结果
            result.Suc = true;
            result.RoomId = receive.RoomId;
            data = NetworkUtils.Serialize(result);
            player.Send(MessageType.CreatRoom, data);
        }
        else
        {
            Console.WriteLine($"玩家:{player.Name}创建房间失败");
            //向客户端发送操作结果
            data = NetworkUtils.Serialize(result);
            player.Send(MessageType.CreatRoom, data);
        }
    }

    private void _EnterRoom(Player player, byte[] data)
    {
        //结果
        EnterRoom result = new EnterRoom();

        EnterRoom receive = NetworkUtils.Deserialize<EnterRoom>(data);

        //逻辑检测(玩家不在任何房间中 并且 存在该房间)
        if (!player.InRoom && Server.Rooms.ContainsKey(receive.RoomId))
        {
            Room room = Server.Rooms[receive.RoomId];
            //加入玩家
            if (room.Players.Count < Room.MAX_PLAYER_AMOUNT && !room.Players.Contains(player))
            {
                room.Players.Add(player);
                player.EnterRoom(receive.RoomId);

                Console.WriteLine($"玩家:{player.Name}成为了房间:{receive.RoomId}的玩家");

                //向玩家发送成功操作结果
                result.RoomId = receive.RoomId;
                result.result = EnterRoom.Result.Player;
                data = NetworkUtils.Serialize(result);
                player.Send(MessageType.EnterRoom, data);
            }
            //加入房间失败
            else
            {
                Console.WriteLine($"玩家:{player.Name}加入房间失败");

                result.result = EnterRoom.Result.None;
                data = NetworkUtils.Serialize(result);
                player.Send(MessageType.EnterRoom, data);
            }
        }
        else
        {
            Console.WriteLine($"玩家:{player.Name}进入房间失败");
            //向玩家发送失败操作结果
            data = NetworkUtils.Serialize(result);
            player.Send(MessageType.EnterRoom, data);
        }
    }

    private void _ExitRoom(Player player, byte[] data)
    {
        //结果
        ExitRoom result = new ExitRoom();

        ExitRoom receive = NetworkUtils.Deserialize<ExitRoom>(data);
        //逻辑检测(有该房间)
        if (Server.Rooms.ContainsKey(receive.RoomId))
        {
            //确保有该房间并且玩家在该房间内
            if (Server.Rooms[receive.RoomId].Players.Contains(player) ||
                Server.Rooms[receive.RoomId].OBs.Contains(player))
            {
                result.Suc = true;
                //移除该玩家
                if (Server.Rooms[receive.RoomId].Players.Contains(player))
                {
                    Server.Rooms[receive.RoomId].Players.Remove(player);
                }
                else if (Server.Rooms[receive.RoomId].OBs.Contains(player))
                {
                    Server.Rooms[receive.RoomId].OBs.Remove(player);
                }

                if (Server.Rooms[receive.RoomId].Players.Count == 0)
                {
                    Server.Rooms.Remove(receive.RoomId); //如果该房间没有玩家则移除该房间
                }

                Console.WriteLine($"玩家:{player.Name}退出房间成功");

                player.ExitRoom();
                //向玩家发送成功操作结果
                data = NetworkUtils.Serialize(result);
                player.Send(MessageType.ExitRoom, data);
            }
            else
            {
                Console.WriteLine($"玩家:{player.Name}退出房间失败");
                //向玩家发送失败操作结果
                data = NetworkUtils.Serialize(result);
                player.Send(MessageType.ExitRoom, data);
            }
        }
        else
        {
            Console.WriteLine($"玩家:{player.Name}退出房间失败");
            //向玩家发送失败操作结果
            data = NetworkUtils.Serialize(result);
            player.Send(MessageType.ExitRoom, data);
        }
    }

    private void _StartGame(Player player, byte[] data)
    {
        //结果
        StartGame result = new StartGame();

        StartGame receive = NetworkUtils.Deserialize<StartGame>(data);

        //逻辑检测(有该房间)
        if (Server.Rooms.ContainsKey(receive.RoomId))
        {
            //玩家模式开始游戏
            if (Server.Rooms[receive.RoomId].Players.Contains(player) &&
                Server.Rooms[receive.RoomId].Players.Count <= Room.MAX_PLAYER_AMOUNT)
            {
                //游戏开始
                Server.Rooms[receive.RoomId].State = Room.RoomState.Gaming;
                data = NetworkUtils.Serialize(result);
                Console.WriteLine($"玩家:{player.Name}开始游戏成功");

                //遍历该房间玩家
                foreach (var each in Server.Rooms[receive.RoomId].Players)
                {
                    
                    result.Suc = true;
/*                    result.First = true;*/
                    result.Name = player.Name;
                    data = NetworkUtils.Serialize(result);
                    each.Send(MessageType.StartGame, data);
                }
                //遍历该房间玩家
                foreach (var each in Server.Rooms[receive.RoomId].Players)
                {
                    if (each.Name == player.Name) continue;
                    result.Suc = true;
/*                    result.First = true;*/
                    result.Name = each.Name;
                    data = NetworkUtils.Serialize(result);
                    player.Send(MessageType.StartGame, data);
                }
            }
            else
            {
                Console.WriteLine($"玩家:{player.Name}开始游戏失败");
                //向玩家发送失败操作结果
                data = NetworkUtils.Serialize(result);
                player.Send(MessageType.StartGame, data);
            }
        }
        else
        {
            Console.WriteLine($"玩家:{player.Name}开始游戏失败");
            //向玩家发送失败操作结果
            data = NetworkUtils.Serialize(result);
            player.Send(MessageType.StartGame, data);
        }
    }
    private void _ExitGame(Player player, byte[] data)
    {
        ExitGame result = new ExitGame();
        ExitGame receive = NetworkUtils.Deserialize<ExitGame>(data);
        result.Name = receive.Name;
        result.RoomId = receive.RoomId;
        result.Suc = true;
        data = NetworkUtils.Serialize(result);
        foreach(var each in Server.Rooms[player.RoomId].Players)
        {
            if (each.Name != result.Name)
            {
                each.Send(MessageType.ExitGame, data);
            }
        }
    }
    private void _PlayMove(Player player,byte[] data)
    {
        Move result = new Move();
        Move receive = NetworkUtils.Deserialize<Move>(data);
        Server.PlayerPosition[player] = receive;
        result.Gamer= receive.Gamer;
        result.RoomId = receive.RoomId;
        result.X = receive.X;
        result.Y = receive.Y;
        result.Z = receive.Z;
        result.Amount = receive.Amount;
        result.Name = receive.Name;
        result.Suc = true;
        data = NetworkUtils.Serialize(result);
        foreach(var each in Server.Rooms[player.RoomId].Players)
        {
            each.Send(MessageType.Move, data);
        }
    }
    private void _PlayVoice(Player player, byte[] data)
    {
        Voice result = new Voice();
        Voice receive = NetworkUtils.Deserialize<Voice>(data);
        result.Gamer = receive.Gamer;
        result.data = receive.data;
        result.Name = receive.Name;
        result.Suc = true;
        data = NetworkUtils.Serialize(result);
        foreach (var each in Server.Rooms[player.RoomId].Players)
        {
            if (each.Name != player.Name)
            {
                each.Send(MessageType.PlayVoice, data);
            }
        }
    }
}