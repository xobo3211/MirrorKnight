using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MirrorKnight
{
    class Room
    {
        int rType;
        int rNo;
        Random rand = new Random();
        public Room(int rT, int rN)
        {
            rType = rT;
            rNo = rN;
        }
        public Room(int rT)
        {
            rType = rT;
            int c;
            switch (rT)
            {
                case (0):
                    c = 10; 
                    break;
                case (1):
                    c = 10;
                    break;
                case (2):
                    c = 10;
                    break;
                case (3):
                    c = 10;
                    break;
                case (4):
                    c = 10;
                    break;
            }
            rNo = rand.Next(0, );
        }
        public int getRoomType()
        {
            return rType;
        }
        public int getRoomNo()
        {
            return rNo;
        }
        public void setRoomType(int r)
        {
            rType = r;
        }
        public void setRoomNo(int r)
        {
            rNo = r;
        }
        
    }
}
