/*
 * Authors: Team 18
 * Itay Bouganim, ID: 305278384
 * Sahar Vaya, ID: 205583453
 * Dayan Badalbaev, ID: 209222215
 */

//Interface to determine local file handeling methods
public interface IHandler<T>
{
    //Interface methods
    void Write(T list);
    T Read();
}
